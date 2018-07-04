using AssignParameterToInstancesInsideBBox.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.BoundingBoxXYZExts;

namespace AssignParameterToInstancesInsideBBox.Repos
{
    public class BoxedElementRepo
    {
        public List<BuiltInCategory> BuiltInCategoryList { get; set; }
        public ExternalCommandData CommandData { get; }
        public BoxedElementRepo(ExternalCommandData commandData, List<BuiltInCategory> builtInCategoryList)
        {
            CommandData = commandData;
            BuiltInCategoryList = builtInCategoryList;
        }

        public List<BoxedElement> GetBoxedInstances(MassFamilyInstance massFamilyInstance)
        {
            var boxedFInstances = new List<BoxedElement>();
            //берем всё экземляры определённой категории
            var elementList = RevitPSVUtils.ElementUtils
                                    .GetElementsOfBuiltInCategoryList(CommandData, BuiltInCategoryList);
            var massFamilyInstanceBox = massFamilyInstance.BoundBox;
            //и если их ббоксы входят в большой ббокс формобразующего семейства,
            //то включаем их в выборку
            foreach (var oElement in elementList)
            {
                var fInstanceBox = oElement.get_BoundingBox(null);
                if (fInstanceBox == null)
                    continue;
                var isFInstanceBBoxInsideMassBBox = fInstanceBox.IsInsideBoundingBox(massFamilyInstanceBox);
                if (isFInstanceBBoxInsideMassBBox)
                {
                    var boxedFInstance = new BoxedElement(massFamilyInstance.Element, oElement);
                    boxedFInstances.Add(boxedFInstance);
                }
            }
            return boxedFInstances;
        }
    }
}
