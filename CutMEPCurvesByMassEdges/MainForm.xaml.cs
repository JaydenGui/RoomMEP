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
using RevitPSVUtils;
using RevitPSVUtils.EnumData;
using Line = Autodesk.Revit.DB.Line;
using System.Xml.Linq;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Electrical;

using static RevitPSVUtils.ElementExts;
using CutMEPCurvesByMassEdges.Models;

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
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            //собираем все линейные объекты и масс-формы по названию

            var massForms = MassFormModel.GetMassFormModels(_commandData, txtboxMassFormName.Text);
            var pipes = PipeModel.GetPipeModels(_commandData);

            //берём масс-формы и трубы, находим точки пересечения
            var pipeAndMassFormIntersectionList = PipeAndMassFormIntersection
                                                    .GetIntersectionList(pipes, massForms);

            //создаём отдельные трубы из труб, которые пересекаются с формообразующими
            foreach (var pipeAndMassInt in pipeAndMassFormIntersectionList)
            {
                var pointsToCreatePipes = new List<XYZ>();
                pointsToCreatePipes.Add(pipeAndMassInt.Pipe.StarPoint);
                pointsToCreatePipes.AddRange(pipeAndMassInt.IntersectionPoints
                                                .OrderBy(p => p.DistanceTo(pipeAndMassInt.Pipe.StarPoint)).ToList());
                pointsToCreatePipes.Add(pipeAndMassInt.Pipe.EndPoint);
                for (int i = 0; i < pointsToCreatePipes.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var currentPoint = pointsToCreatePipes[i];
                    var prevPoint = pointsToCreatePipes[i - 1];
                    if (currentPoint.IsEqualByXYZ(prevPoint))
                        continue;
                    PipesUtils.CreateNewPipeByTypeOfExisted(
                        pipeAndMassInt.Pipe.Model, currentPoint, prevPoint, _commandData);
                }
                DeleteUtils.DeleteElements(_commandData, new List<Element> { pipeAndMassInt.Pipe.Model });
            }
            this.Close();
            return;
        }
    }
}
