using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB.Structure;
using RevitPSVUtils;
using System.Xml.Linq;
using AssignParameterToInstancesInsideBBox.Repos;

namespace AssignParameterToInstancesInsideBBox
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
        private string xmlDocUserInputFilePath = string.Empty;
        private const string xmlAutoSettingsFileName = "MEPRoomAutoSettings.xml";

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

            //задаём путь сохранения настроек
            xmlDocUserInputFilePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                xmlAutoSettingsFileName);

            var xmlDealer = new XMLDealer();
            xmlDealer.GetXMLDataToWPF(xmlDocUserInputFilePath,
                ref txtbox_massFamilyName, ref txtbox_massParameterName,
                ref txtbox_instancesInsideMassParameterName);
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            //собираем все экземпляры формообразующих семейств и семейств по категориям
            var massFamilyRepo = new MassFamilyRepo(_commandData);
            var massFInstanceList = massFamilyRepo.GetInstancesByName(txtbox_massFamilyName.Text);
            if (massFInstanceList.Count == 0)
            {
                MessageBox.Show($"Не найдено семейство с именем {txtbox_massFamilyName.Text}");
                return;
            }
            var categoryList = new List<BuiltInCategory> {
                                                            BuiltInCategory.OST_CableTray,
                                                            BuiltInCategory.OST_CableTrayFitting,
                                                            BuiltInCategory.OST_Conduit,
                                                            BuiltInCategory.OST_ConduitFitting,
                                                            BuiltInCategory.OST_DuctCurves,
                                                            BuiltInCategory.OST_DuctFitting,
                                                            BuiltInCategory.OST_DuctTerminal,
                                                            BuiltInCategory.OST_ElectricalEquipment,
                                                            BuiltInCategory.OST_ElectricalFixtures,
                                                            BuiltInCategory.OST_LightingDevices,
                                                            BuiltInCategory.OST_LightingFixtures,
                                                            BuiltInCategory.OST_MechanicalEquipment,
                                                            BuiltInCategory.OST_PipeCurves,
                                                            BuiltInCategory.OST_PipeFitting,
                                                            BuiltInCategory.OST_PlumbingFixtures,
                                                            BuiltInCategory.OST_SpecialityEquipment,
                                                            BuiltInCategory.OST_Sprinklers,
                                                            BuiltInCategory.OST_Wire};
            var boxedFamilyRepo = new BoxedElementRepo(_commandData, categoryList);

            foreach (var massFInstance in massFInstanceList)
            {
                string massParamStringValue = string.Empty;
                double massParamDoubleValue = 0;
                int massParamIntValue = 0;
                var IsMassParamCorrect = massFInstance.Element.GetParameterValue(txtbox_massParameterName.Text,
                     ref massParamStringValue, ref massParamDoubleValue, ref massParamIntValue);
                if (!IsMassParamCorrect)
                {
                    MessageBox.Show($"Не удалось получить значение параметра для семейства с ID {massFInstance.Element.Id}");
                    return;
                }
                var boxedFInstances = boxedFamilyRepo.GetBoxedInstances(massFInstance);
                foreach (var boxedFInstance in boxedFInstances)
                {
                    var boxedFInstanceParam = boxedFInstance.Element
                                                            .LookupParameter(txtbox_instancesInsideMassParameterName.Text);
                    if (boxedFInstanceParam == null)
                    {
                        continue;
                    }
                    using (var t = new Transaction(doc, "Set param"))
                    {
                        t.Start();
                        if (boxedFInstanceParam.StorageType == StorageType.String)
                            boxedFInstanceParam.Set(massParamStringValue);
                        else if (boxedFInstanceParam.StorageType == StorageType.Double)
                            boxedFInstanceParam.Set(massParamDoubleValue);
                        else if (boxedFInstanceParam.StorageType == StorageType.Integer)
                            boxedFInstanceParam.Set(massParamIntValue);
                        t.Commit();
                    }
                }
            }
            this.Close();
            return;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var xmlDealer = new XMLDealer();
            xmlDealer.SaveUserInputToXML(xmlDocUserInputFilePath,
                txtbox_massFamilyName, txtbox_massParameterName,
                txtbox_instancesInsideMassParameterName);
        }
    }
}
