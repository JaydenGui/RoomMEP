using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.DuctExts;
using static RevitPSVUtils.MEPCurveExts;

namespace CutMEPCurvesByMassEdges.Models
{
    public class DuctModel : MEPLineObject
    {
        public Duct Model { get; set; }

        public static DuctModel Create(Duct oDuct)
        {
            var ductStartEndPoints = oDuct.GetStartAndEndPoint();
            var ductConnectors = oDuct.GetConnectors();

            if (ductStartEndPoints.Count < 2 || ductConnectors.Count < 2)
                return null;

            var ductModel = new DuctModel
            {
                Model = oDuct,
                Curve = oDuct.GetCurve(),
                StarPoint = ductStartEndPoints[0],
                EndPoint = ductStartEndPoints[1],
                ConnectorFirst = ductConnectors[0],
                ConnectorSecond = ductConnectors[1]

            };

            return ductModel;
        }
    }
}
