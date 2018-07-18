using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Autodesk.Revit.DB.Structure;
using Line = Autodesk.Revit.DB.Line;
using System.Xml.Linq;
using RevitPSVUtils;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using AssignPipeRiseParameter.Model;

namespace AssignPipeRiseParameter
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

        private string xmlAutoSettingsFileName = "AssignPipeRiseParameter.xml";
        private string xmlDocUserInputFilePath = string.Empty;

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

            #region Забираем сохранённые данные из XML-файла
            //задаём путь сохранения настроек
            xmlDocUserInputFilePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                xmlAutoSettingsFileName);

            var xmlDealer = new XMLDealer();
            xmlDealer.GetXMLDataToWPF(xmlDocUserInputFilePath,
                ref txtboxParameterRise, ref txtboxRiseHeight);
            #endregion

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var xmlDealer = new XMLDealer();
            xmlDealer.SaveUserInputToXML(xmlDocUserInputFilePath, txtboxParameterRise, txtboxRiseHeight);
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            //Собираем все элементы
            var ducts = GenericSelectionUtils<Duct>.GetObjectsByType(_commandData).Cast<Element>().ToList();
            var pipes = GenericSelectionUtils<Pipe>.GetObjectsByType(_commandData).Cast<Element>().ToList();
            var cableTrays = GenericSelectionUtils<CableTray>.GetObjectsByType(_commandData).Cast<Element>().ToList();
            var conduits = GenericSelectionUtils<Conduit>.GetObjectsByType(_commandData).Cast<Element>().ToList();

            var heightParsedValue = NumberUtils.ParseStringToDouble(txtboxRiseHeight.Text);
            if (heightParsedValue == 0)
            {
                this.Close();
                return;
            }

            var heightMaxFeet = NumberUtils.MillimetersToFeet(heightParsedValue);

            //Записываем параметр
            var parameterRiseName = txtboxParameterRise.Text;
            var parameterRiseValue = "Стояк";

            var ductsRises = new MEPRise().CreateMEPLines(ducts, heightMaxFeet).Select(d => d.Model).ToList();
            var pipeRises = new MEPRise().CreateMEPLines(pipes, heightMaxFeet).Select(d => d.Model).ToList();
            var cableTrayRises = new MEPRise().CreateMEPLines(cableTrays, heightMaxFeet).Select(d => d.Model).ToList();
            var conduitRises = new MEPRise().CreateMEPLines(conduits, heightMaxFeet).Select(d => d.Model).ToList();

            ElementUtils.SetParameterValueToElementList(_commandData, ductsRises, parameterRiseName, parameterRiseValue);
            ElementUtils.SetParameterValueToElementList(_commandData, pipeRises, parameterRiseName, parameterRiseValue);
            ElementUtils.SetParameterValueToElementList(_commandData, cableTrayRises, parameterRiseName, parameterRiseValue);
            ElementUtils.SetParameterValueToElementList(_commandData, conduitRises, parameterRiseName, parameterRiseValue);

            this.Close();
            return;
        }
    }
}
