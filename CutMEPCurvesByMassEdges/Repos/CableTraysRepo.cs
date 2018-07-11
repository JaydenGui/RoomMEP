using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using CutMEPCurvesByMassEdges.Models;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class CableTraysRepo
    {
        public static List<CableTrayModel> GetCableTrayModels(ExternalCommandData commandData)
        {
            var condModelList = new List<CableTrayModel>();
            var cableTrays = GenericSelectionUtils<CableTray>.GetObjectsByType(commandData);
            foreach (var oCableTray in cableTrays)
            {
                var startEndPoints = oCableTray.GetStartAndEndPoint();
                var connectors = oCableTray.GetConnectors();

                if (startEndPoints.Count < 2 || connectors.Count < 2)
                    continue;

                var condModel = new CableTrayModel
                {
                    Model = oCableTray,
                    Curve = oCableTray.GetCurve(),
                    StarPoint = startEndPoints[0],
                    EndPoint = startEndPoints[1],
                    ConnectorFirst = connectors[0],
                    ConnectorSecond = connectors[1]
                };
                condModelList.Add(condModel);
            }
            return condModelList;
        }
    }
}
