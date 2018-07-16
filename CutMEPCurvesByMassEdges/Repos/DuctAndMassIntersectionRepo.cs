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
        private static DuctAndMassIntersectionModel GetIntersection(DuctModel DuctModel, MassFormModel massFormModel)
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

        public static List<DuctAndMassIntersectionModel> GetIntersectionList(List<DuctModel> DuctModelList,
            List<MassFormModel> massFormModelList)
        {
            var DuctAndMassFormIntersectionList = new List<DuctAndMassIntersectionModel>();

            foreach (var massFormItem in massFormModelList)
            {
                foreach (var oDuct in DuctModelList)
                {

                    var DuctAndMassFormIntersection = GetIntersection(oDuct, massFormItem);
                    if (DuctAndMassFormIntersection == null)
                        continue;
                    DuctAndMassFormIntersectionList.Add(DuctAndMassFormIntersection);
                }
            }
            return DuctAndMassFormIntersectionList;
        }
    }
}
