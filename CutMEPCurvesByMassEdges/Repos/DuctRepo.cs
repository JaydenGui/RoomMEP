using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using CutMEPCurvesByMassEdges.Models;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.DuctExts;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class DuctRepo
    {
        public static List<DuctModel> GetDuctModels(ExternalCommandData commandData)
        {
            var ductsModel = new List<DuctModel>();
            var ducts = GenericSelectionUtils<Duct>.GetObjectsByType(commandData);
            foreach (var oDuct in ducts)
            {
                var pipeStartEndPoints = oDuct.GetStartAndEndPoint();
                var pipeConnectors = oDuct.GetConnectors();

                if (pipeStartEndPoints.Count < 2 || pipeConnectors.Count < 2)
                    continue;

                var pipeModel = new DuctModel
                {
                    Model = oDuct,
                    Curve = oDuct.GetCurve(),
                    StarPoint = pipeStartEndPoints[0],
                    EndPoint = pipeStartEndPoints[1],
                    ConnectorFirst = pipeConnectors[0],
                    ConnectorSecond = pipeConnectors[1]
                };
                ductsModel.Add(pipeModel);
            }
            return ductsModel;
        }
    }
}
