using Autodesk.Revit.DB;
using CutMEPCurvesByMassEdges.Models;
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
            var DuctCurveStartPoint = DuctModel.StarPoint;
            var DuctCurveEndPoint = DuctModel.EndPoint;

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
                    //проверяем находится ли точка на линии
                    if (GeomShark.PointUtils.IsPointBetweenOtherTwoPoints(
                                                            DuctCurveStartPoint.X, DuctCurveStartPoint.Y,
                                                            DuctCurveEndPoint.X, DuctCurveEndPoint.Y,
                                                            intersectPoint.X, intersectPoint.Y, 4))
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
