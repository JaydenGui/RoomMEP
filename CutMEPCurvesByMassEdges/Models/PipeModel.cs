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
    public class PipeModel : MEPLineObject
    {
        public Pipe Model { get; set; }

        public static PipeModel Create(Pipe oPipe)
        {
            var pipeStartEndPoints = oPipe.GetStartAndEndPoint();
            var pipeConnectors = oPipe.GetConnectors();

            if (pipeStartEndPoints.Count < 2 || pipeConnectors.Count < 2)
                return null;

            var pipeModel = new PipeModel
            {
                Model = oPipe,
                Curve = oPipe.GetPipeCurve(),
                StarPoint = pipeStartEndPoints[0],
                EndPoint = pipeStartEndPoints[1],
                ConnectorFirst = pipeConnectors[0],
                ConnectorSecond = pipeConnectors[1]
            };

            return pipeModel;
        }
    }
}
