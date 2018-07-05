using AssignParameterToInstancesInsideBBox.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.BoundingBoxXYZExts;
using static RevitPSVUtils.XYZUtils;
using static RevitPSVUtils.ElementExts;
using static RevitPSVUtils.NumberUtils;
using MoreLinq;

namespace AssignParameterToInstancesInsideBBox.Repos
{
    public class BoxedElementRepo
    {
        //public List<BuiltInCategory> BuiltInCategoryList { get; set; }
        public ExternalCommandData CommandData { get; }
        public BoxedElementRepo(ExternalCommandData commandData/*, List<BuiltInCategory> builtInCategoryList*/)
        {
            CommandData = commandData;
            //BuiltInCategoryList = builtInCategoryList;
        }

        public List<BoxedElement> GetBoxedInstances(MassFamilyInstance massFamilyInstance)
        {
            var boxedFInstances = new List<BoxedElement>();
            //берем всё экземляры определённой категории
            var elementList = RevitPSVUtils.ElementUtils
                                    .GetModelElements(massFamilyInstance.Doc)
                                    .Where(e => e.get_BoundingBox(null) != null &&
                                                    e.Category != null &&
                                                    e.Location != null)
                                    .ToList();
            //берем точки граней масс семейства
            var massFacePointsList = massFamilyInstance.GetFacePoints();

            foreach (var oElement in elementList)
            {
                if (oElement.Equals(massFamilyInstance.Element))
                    continue;
                var elementBoundBox = oElement.get_BoundingBox(null);
                if (elementBoundBox == null)
                    continue;
                var elementMidPoint = elementBoundBox.MiddlePointXYZ();
                if (elementMidPoint == null)
                    continue;
                bool isElementPointsInsideMass = false;

                //Элемент должен быть внутри всех граней (2д полигонов)
                //чтобы мы точно сказали, что он входит в данную масс форму
                int elementInsidePolygonCount = 0;
                foreach (var massFacePoints in massFacePointsList)
                {
                    //Проверяем находится ли средняя точка элемента внутри грани масс формы
                    var isPointOfElementInsidePolygon = IsInPolygonBy2d(
                            massFacePoints, elementMidPoint);
                    //если точка элемента внутри полигона, то добавляем значение к счетчику
                    if (isPointOfElementInsidePolygon)
                    {
                        elementInsidePolygonCount++;
                    }
                    //иначе завершаем цикл
                    else
                    {
                        break;
                    }
                }


                //если количество полигонов, в котором находится элемент
                //равно количество граней, то элемент находится внутри масс формы
                if (elementInsidePolygonCount == massFacePointsList.Count)
                    isElementPointsInsideMass = true;

                //находим макс и мин высотные точки Z масс-формы
                var massFacePointsToOneList = massFacePointsList.SelectMany(x => x).ToList();
                var massMaxZCoord = massFacePointsToOneList.MaxBy(x => x.Z).Select(x => x.Z).First();
                var massMinZCoord = massFacePointsToOneList.MinBy(x => x.Z).Select(x => x.Z).First();


                //проверяем находится ли элемент в пределах мин и макс значений 
                if (!IsInRange(elementMidPoint.Z, massMinZCoord, massMaxZCoord))
                    //и если элемент не в диапазоне, то улетаем
                    isElementPointsInsideMass = false;

                //Если элемент находится внутри масс-формы, то добавляем его
                if (isElementPointsInsideMass)
                {
                    var boxedFInstance = new BoxedElement(massFamilyInstance.Element, oElement);
                    boxedFInstances.Add(boxedFInstance);
                }
            }
            return boxedFInstances;
        }
    }
}
