using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.MEPCurveExts;


namespace CutMEPCurvesByMassEdges.Models
{
    public class CableTrayModel : MEPLineObject
    {
        public CableTray Model { get; set; }

        public static CableTrayModel Create(CableTray oCableTray)
        {
            var curveStartEndPoint = oCableTray.GetStartAndEndPoint();
            var connectors = oCableTray.GetConnectors();

            if (curveStartEndPoint.Count < 2 || connectors.Count < 2)
                return null;

            var pipeModel = new CableTrayModel
            {
                Model = oCableTray,
                Curve = oCableTray.GetCurve(),
                StarPoint = curveStartEndPoint[0],
                EndPoint = curveStartEndPoint[1],
                ConnectorFirst = connectors[0],
                ConnectorSecond = connectors[1]
            };

            return pipeModel;
        }
    }
}
