using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
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
    public class ConduitRepo
    {
        public static List<ConduitModel> GetConduitModels(ExternalCommandData commandData)
        {
            var condModelList = new List<ConduitModel>();
            var conduits = GenericSelectionUtils<Conduit>.GetObjectsByType(commandData);
            foreach (var oConduit in conduits)
            {
                var startEndPoints = oConduit.GetStartAndEndPoint();
                var connectors = oConduit.GetConnectors();

                if (startEndPoints.Count < 2 || connectors.Count < 2)
                    continue;

                var condModel = new ConduitModel
                {
                    Model = oConduit,
                    Curve = oConduit.GetCurve(),
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
