using Autodesk.Revit.DB;
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
    public class FittingRepo
    {
        public List<FittingModel> GetFittingsFromModel(ExternalCommandData commandData, BuiltInCategory category)
        {
            var fittingModels = new List<FittingModel>();

            var fittings = ElementUtils
                .GetElementsOfBuiltInCategory(commandData, category);
            foreach (var fittingElem in fittings)
            {
                var fitting = fittingElem as FamilyInstance;
                var fittingConnectorPoints = fitting.GetConnectorPoints();
                if (fittingConnectorPoints == null || fittingConnectorPoints.Count == 0)
                    return null;
                var fittingModel = new FittingModel
                {
                    ConnectorPoints = fittingConnectorPoints,
                    Model = fitting
                };
                fittingModels.Add(fittingModel);
            }
            return fittingModels;
        }

        public List<XYZ> GetConnectorPoints(ExternalCommandData commandData, BuiltInCategory category)
        {
            var fittings = GetFittingsFromModel(commandData, category);
            var points = fittings.SelectMany(f => f.ConnectorPoints).ToList();
            return points;
        }
    }
}
