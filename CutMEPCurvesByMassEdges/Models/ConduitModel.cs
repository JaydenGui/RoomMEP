using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.MEPCurveExts;

namespace CutMEPCurvesByMassEdges.Models
{
    public class ConduitModel : MEPLineObject
    {
        public Conduit Model { get; set; }

        public static ConduitModel Create(Conduit oConduit)
        {
            var curveStartEndPoint = oConduit.GetStartAndEndPoint();
            var connectors = oConduit.GetConnectors();

            if (curveStartEndPoint.Count < 2 || connectors.Count < 2)
                return null;

            var pipeModel = new ConduitModel
            {
                Model = oConduit,
                Curve = oConduit.GetCurve(),
                StarPoint = curveStartEndPoint[0],
                EndPoint = curveStartEndPoint[1],
                ConnectorFirst = connectors[0],
                ConnectorSecond = connectors[1]
            };

            return pipeModel;
        }
    }
}
