using Autodesk.Revit.DB.Plumbing;
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
    public class PipeRepo
    {
        public static List<PipeModel> GetPipeModels(ExternalCommandData commandData)
        {
            var pipesModel = new List<PipeModel>();
            var pipes = GenericSelectionUtils<Pipe>.GetObjectsByType(commandData);
            foreach (var oPipe in pipes)
            {
                var pipeStartEndPoints = oPipe.GetStartAndEndPoint();
                var pipeConnectors = oPipe.GetConnectors();

                if (pipeStartEndPoints.Count < 2 || pipeConnectors.Count < 2)
                    continue;

                var pipeModel = new PipeModel
                {
                    Model = oPipe,
                    Curve = oPipe.GetCurve(),
                    StarPoint = pipeStartEndPoints[0],
                    EndPoint = pipeStartEndPoints[1],
                    ConnectorFirst = pipeConnectors[0],
                    ConnectorSecond = pipeConnectors[1]
                };
                pipesModel.Add(pipeModel);
            }
            return pipesModel;
        }

        
    }
}
