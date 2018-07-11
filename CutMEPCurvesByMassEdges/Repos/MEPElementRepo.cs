using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CutMEPCurvesByMassEdges.Models;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.FamilyInstanceExts;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class MEPElementRepo
    {
        public static List<MEPElementModel> GetMEPElementsFromModel(ExternalCommandData _commandData)
        {
            //Выбираем все MEP элементы, где есть коннекторы
            var mepElements = GenericSelectionUtils<FamilyInstance>
                .GetObjectsByType(_commandData)
                .Where(o => o.MEPModel.ConnectorManager != null);
            var mepElementsModel = new List<MEPElementModel>();

            foreach (var mepElementItem in mepElements)
            {
                var newMEPModel = new MEPElementModel
                {
                    Model = mepElementItem,
                    Connectors = mepElementItem.GetConnectors()
                };
                mepElementsModel.Add(newMEPModel);
            }
            return mepElementsModel;
        }
    }
}
