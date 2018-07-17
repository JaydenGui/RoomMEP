using Autodesk.Revit.DB;
using CutMEPCurvesByMassEdges.Models;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class DuctAndMassIntersectionRepo
    {
        private static DuctAndMassIntersectionModel GetIntersection(
            DuctModel DuctModel, MassFormModel massFormModel, List<XYZ> fittingPointList)
        {
            var intersectionsInstance = new DuctAndMassIntersectionModel { Duct = DuctModel, MassForm = massFormModel };
            var ductCurveStartPoint = DuctModel.StarPoint;
            var ductCurveEndPoint = DuctModel.EndPoint;

            foreach (var massFace in massFormModel.Faces)
            {
                IntersectionResultArray intersectionResultArray = null;
                massFace.Intersect(DuctModel.Curve, out intersectionResultArray);
                if (intersectionResultArray == null)
                    continue;

                foreach (IntersectionResult intResult in intersectionResultArray)
                {
                    if (intResult.XYZPoint == null)
                        continue;

                    var intersectPoint = intResult.XYZPoint;

                    //Проверяем совпадает ли точка пересечения с точкой коннектора фитинга
                    bool isFittingPointMatch = false;
                    foreach (var fittingPoint in fittingPointList)
                    {
                        if (intersectPoint.IsEqualByXYZ(fittingPoint, 5))
                        {
                            isFittingPointMatch = true;
                            break;
                        }
                    }

                    if (isFittingPointMatch)
                        continue;

                    //Если точка коннектора трубы совпадает с точкой пересечения, то улетаем
                    if (intersectPoint.IsEqualByXYZ(ductCurveStartPoint, 5) ||
                        intersectPoint.IsEqualByXYZ(ductCurveEndPoint, 5))
                        continue;

                    bool isIntersectPointInRange
                        = NumberUtils.IsInRange(
                            intersectPoint.Z,
                            Math.Min(ductCurveStartPoint.Z, ductCurveStartPoint.Z),
                            Math.Max(ductCurveEndPoint.Z, ductCurveEndPoint.Z));

                    //проверяем находится ли точка на линии
                    if (GeomShark.PointUtils.IsPointBetweenOtherTwoPoints(
                                                            ductCurveStartPoint.X, ductCurveStartPoint.Y,
                                                            ductCurveEndPoint.X, ductCurveEndPoint.Y,
                                                            intersectPoint.X, intersectPoint.Y, 4) &&
                        isIntersectPointInRange)
                    {
                        intersectionsInstance.IntersectionPoints.Add(intersectPoint);
                    }
                }
            }

            return (intersectionsInstance.IntersectionPoints.Count > 0) ? intersectionsInstance : null;
        }

        public static List<DuctAndMassIntersectionModel> GetIntersectionList(
            List<DuctModel> DuctModelList,
            List<MassFormModel> massFormModelList,
            List<XYZ> fittingPointList)
        {
            var ductAndMassFormIntersectionList = new List<DuctAndMassIntersectionModel>();

            foreach (var massFormItem in massFormModelList)
            {
                foreach (var oDuct in DuctModelList)
                {

                    var ductAndMassFormIntersection = GetIntersection(oDuct, massFormItem, fittingPointList);
                    if (ductAndMassFormIntersection == null)
                        continue;
                    ductAndMassFormIntersectionList.Add(ductAndMassFormIntersection);
                }
            }
            return ductAndMassFormIntersectionList;
        }
    }
}
