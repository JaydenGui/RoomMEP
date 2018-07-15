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
    public class ConduitAndMassIntersectionRepo
    {
        private static ConduitAndMassIntersectionModel GetIntersection(ConduitModel ConduitModel, MassFormModel massFormModel)
        {
            var intersectionsInstance = new ConduitAndMassIntersectionModel { Conduit = ConduitModel, MassForm = massFormModel };
            var conduitCurveStartPoint = ConduitModel.StarPoint;
            var conduitCurveEndPoint = ConduitModel.EndPoint;

            foreach (var massFace in massFormModel.Faces)
            {
                IntersectionResultArray intersectionResultArray = null;
                massFace.Intersect(ConduitModel.Curve, out intersectionResultArray);
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
                            Math.Min(conduitCurveStartPoint.Z, conduitCurveStartPoint.Z),
                            Math.Max(conduitCurveEndPoint.Z, conduitCurveEndPoint.Z));

                    //проверяем находится ли точка на линии
                    if (GeomShark.PointUtils.IsPointBetweenOtherTwoPoints(
                                                            conduitCurveStartPoint.X, conduitCurveStartPoint.Y,
                                                            conduitCurveEndPoint.X, conduitCurveEndPoint.Y,
                                                            intersectPoint.X, intersectPoint.Y, 4))
                    {
                        intersectionsInstance.IntersectionPoints.Add(intersectPoint);
                    }
                }
            }

            return (intersectionsInstance.IntersectionPoints.Count > 0) ? intersectionsInstance : null;
        }

        public static List<ConduitAndMassIntersectionModel> GetIntersectionList(List<ConduitModel> ConduitModelList,
            List<MassFormModel> massFormModelList)
        {
            var ConduitAndMassFormIntersectionList = new List<ConduitAndMassIntersectionModel>();

            foreach (var massFormItem in massFormModelList)
            {
                foreach (var oConduit in ConduitModelList)
                {

                    var ConduitAndMassFormIntersection = GetIntersection(oConduit, massFormItem);
                    if (ConduitAndMassFormIntersection == null)
                        continue;
                    ConduitAndMassFormIntersectionList.Add(ConduitAndMassFormIntersection);
                }
            }
            return ConduitAndMassFormIntersectionList;
        }
    }
}
