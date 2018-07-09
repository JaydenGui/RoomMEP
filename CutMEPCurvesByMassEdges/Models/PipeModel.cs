using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Models
{
    public class PipeModel
    {
        public Pipe Model { get; set; }
        public Curve Curve { get; set; }
        public XYZ StarPoint { get; set; }
        public XYZ EndPoint { get; set; }

        public static List<PipeModel> GetPipeModels(ExternalCommandData commandData)
        {
            var pipesModel = new List<PipeModel>();
            var pipes = GenericSelectionUtils<Pipe>.GetObjectsByType(commandData);
            foreach (var oPipe in pipes)
            {
                var pipeStartEndPoints = oPipe.GetStartAndEndPoint();
                if(pipeStartEndPoints.Count < 2)
                    continue;
                var pipeModel = new PipeModel
                {
                    Model = oPipe,
                    Curve = oPipe.GetPipeCurve(),
                    StarPoint = pipeStartEndPoints[0],
                    EndPoint = pipeStartEndPoints[1]
                };
                pipesModel.Add(pipeModel);
            }
            return pipesModel;
        }
    }
}
