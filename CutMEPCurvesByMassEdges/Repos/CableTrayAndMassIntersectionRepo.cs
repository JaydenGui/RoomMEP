using Autodesk.Revit.DB;
using CutMEPCurvesByMassEdges.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class CableTrayAndMassIntersectionRepo
    {
        private static CableTrayAndMassIntersectionModel GetIntersection(CableTrayModel CableTrayModel, MassFormModel massFormModel)
        {
            var intersectionsInstance = new CableTrayAndMassIntersectionModel
            {
                CableTray = CableTrayModel,
                MassForm = massFormModel
            };

            var CableTrayCurveStartPoint = CableTrayModel.StarPoint;
            var CableTrayCurveEndPoint = CableTrayModel.EndPoint;

            foreach (var massFace in massFormModel.Faces)
            {
                IntersectionResultArray intersectionResultArray = null;
                massFace.Intersect(CableTrayModel.Curve, out intersectionResultArray);
                if (intersectionResultArray == null)
                    continue;
                foreach (IntersectionResult intResult in intersectionResultArray)
                {
                    if (intResult.XYZPoint == null)
                        continue;

                    var intersectPoint = intResult.XYZPoint;
                    //проверяем находится ли точка на линии
                    if (GeomShark.PointUtils.IsPointBetweenOtherTwoPoints(
                                                            CableTrayCurveStartPoint.X, CableTrayCurveStartPoint.Y,
                                                            CableTrayCurveEndPoint.X, CableTrayCurveEndPoint.Y,
                                                            intersectPoint.X, intersectPoint.Y, 4))
                    {
                        intersectionsInstance.IntersectionPoints.Add(intersectPoint);
                    }
                }
            }

            return (intersectionsInstance.IntersectionPoints.Count > 0) ? intersectionsInstance : null;
        }

        public static List<CableTrayAndMassIntersectionModel> GetIntersectionList(List<CableTrayModel> CableTrayModelList,
            List<MassFormModel> massFormModelList)
        {
            var CableTrayAndMassFormIntersectionList = new List<CableTrayAndMassIntersectionModel>();

            foreach (var massFormItem in massFormModelList)
            {
                foreach (var oCableTray in CableTrayModelList)
                {

                    var CableTrayAndMassFormIntersection = GetIntersection(oCableTray, massFormItem);
                    if (CableTrayAndMassFormIntersection == null)
                        continue;
                    CableTrayAndMassFormIntersectionList.Add(CableTrayAndMassFormIntersection);
                }
            }
            return CableTrayAndMassFormIntersectionList;
        }
    }
}
