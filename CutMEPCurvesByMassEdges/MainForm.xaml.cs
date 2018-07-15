using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using CutMEPCurvesByMassEdges.Repos;
using System;
using System.Xml.Linq;
using static TextFilesDealer.DirectoryUtils;
using System.IO;

namespace CutMEPCurvesByMassEdges
{
    /// <summary>
    /// Interaction logic for MainUC.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private readonly ExternalCommandData _commandData = null;
        private UIApplication uiapp;
        private UIDocument uidoc;
        private Document doc;
        private string xmlDocUserInputFilePath;
        private string xmlAutoSettingsFileName = "CutMEPCurvesByMass.xml";

        public MainForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            _commandData = commandData;
            Loaded += MainUC_Loaded;
        }

        private void MainUC_Loaded(object sender, RoutedEventArgs e)
        {
            //Дальнейшая инициация приложения
            uiapp = _commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            xmlDocUserInputFilePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                xmlAutoSettingsFileName);

            var oXmlDealer = new XMLDealer();
            oXmlDealer.GetXMLDataToWPF(xmlDocUserInputFilePath, ref txtboxMassFormName);
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            //собираем все линейные объекты и масс-формы по названию

            var massForms = MassFormRepo.GetMassFormModels(_commandData, txtboxMassFormName.Text);
            if (massForms.Count == 0)
            {
                MessageBox.Show($"Не найдены формобразующие с именем семейства {txtboxMassFormName.Text}. ");
                return;
            }
            //И собираем все MEP элементы со всеми коннекторами
            var mepElements = MEPElementRepo.GetMEPElementsFromModel(_commandData);

            #region Трубы
            var pipes = PipeRepo.GetPipeModels(_commandData);

            //берём масс-формы и трубы, находим точки пересечения
            var pipeAndMassFormIntersectionList = PipeAndMassIntersectionRepo
                                                    .GetIntersectionList(pipes, massForms);

            //создаём отдельные трубы из труб, которые пересекаются с формообразующими
            var pipeFactory = new PipesFactoryRepo();
            pipeFactory.CreatePipesFromIntersectionPoints(pipeAndMassFormIntersectionList, _commandData);

            //Берем новые трубы и соединяем их с фитингами, арматурой и приборами.
            //Поскольку при создании труб заново, они отсоединялись
            PipesFactoryRepo.ConnectPipesAndMEPElementsWithConnectorsInSameLocation(
                                                    pipeFactory.Pipes, mepElements, _commandData);

            #endregion

            #region Воздуховоды
            var ducts = DuctRepo.GetDuctModels(_commandData);

            //берём масс-формы и трубы, находим точки пересечения
            var ductAndMassFormIntersectionList = DuctAndMassIntersectionRepo
                                                    .GetIntersectionList(ducts, massForms);

            //создаём отдельные трубы из труб, которые пересекаются с формообразующими
            var ductFactory = new DuctsFactoryRepo();
            ductFactory.CreateDuctsFromIntersectionPoints(ductAndMassFormIntersectionList, _commandData);

            //Берем новые трубы и соединяем их с фитингами, арматурой и приборами.
            //Поскольку при создании труб заново, они отсоединяелись
            DuctsFactoryRepo.ConnectPipesAndMEPElementsWithConnectorsInSameLocation(
                                                    ductFactory.Ducts, mepElements, _commandData);
            #endregion

            #region Короба

            var conduits = ConduitRepo.GetConduitModels(_commandData);

            //берём масс-формы и трубы, находим точки пересечения
            var conduitAndMassFormIntersectionList = ConduitAndMassIntersectionRepo
                                                    .GetIntersectionList(conduits, massForms);

            //создаём отдельные трубы из труб, которые пересекаются с формообразующими
            var conduitFactory = new ConduitsFactoryRepo();
            conduitFactory.CreateConduitsFromIntersectionPoints(conduitAndMassFormIntersectionList, _commandData);

            //Берем новые трубы и соединяем их с фитингами, арматурой и приборами.
            //Поскольку при создании труб заново, они отсоединяелись
            ConduitsFactoryRepo.ConnectConduitsAndMEPElementsWithConnectorsInSameLocation(
                                                    conduitFactory.Conduits, mepElements, _commandData);

            #endregion

            #region Лотки

            var cableTrays = CableTraysRepo.GetCableTrayModels(_commandData);

            //берём масс-формы и трубы, находим точки пересечения
            var cableTrayAndMassFormIntersectionList = CableTrayAndMassIntersectionRepo
                                                    .GetIntersectionList(cableTrays, massForms);

            //создаём отдельные трубы из труб, которые пересекаются с формообразующими
            var cableTrayFactory = new CableTraysFactoryRepo();
            cableTrayFactory.CreateCableTraysFromIntersectionPoints(cableTrayAndMassFormIntersectionList, _commandData);

            //Берем новые трубы и соединяем их с фитингами, арматурой и приборами.
            //Поскольку при создании труб заново, они отсоединяелись
            CableTraysFactoryRepo.ConnectCableTraysAndMEPElementsWithConnectorsInSameLocation(
                                                    cableTrayFactory.CableTrays, mepElements, _commandData);
            #endregion

            this.Close();
            return;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var xmlDealer = new XMLDealer();
            xmlDealer.SaveUserInputToXML(xmlDocUserInputFilePath, txtboxMassFormName);
        }
    }
}
