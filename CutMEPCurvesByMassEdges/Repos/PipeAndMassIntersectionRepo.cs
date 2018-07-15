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
    public class PipeAndMassIntersectionRepo
    {
        private static PipeAndMassIntersectionModel GetIntersection(PipeModel pipeModel, MassFormModel massFormModel)
        {
            var intersectionsInstance = new PipeAndMassIntersectionModel { Pipe = pipeModel, MassForm = massFormModel };
            var pipeCurveStartPoint = pipeModel.StarPoint;
            var pipeCurveEndPoint = pipeModel.EndPoint;

            foreach (var massFace in massFormModel.Faces)
            {
                IntersectionResultArray intersectionResultArray = null;
                massFace.Intersect(pipeModel.Curve, out intersectionResultArray);
                if (intersectionResultArray == null)
                    continue;
                foreach (IntersectionResult intResult in intersectionResultArray)
                {
                    if (intResult.XYZPoint == null)
                        continue;
                    var intersectPoint = intResult.XYZPoint;
                    //проверяем находится ли точка на линии по ХУ и по высотной отметке Z

                    bool isIntersectPointInRange
                        = NumberUtils.IsInRange(
                            intersectPoint.Z,
                            Math.Min(pipeCurveStartPoint.Z, pipeCurveEndPoint.Z),
                            Math.Max(pipeCurveStartPoint.Z, pipeCurveEndPoint.Z));

                    if (GeomShark.PointUtils.IsPointBetweenOtherTwoPoints(
                                                            pipeCurveStartPoint.X, pipeCurveStartPoint.Y,
                                                            pipeCurveEndPoint.X, pipeCurveEndPoint.Y,
                                                            intersectPoint.X, intersectPoint.Y, 4) &&
                         isIntersectPointInRange)
                    {
                        intersectionsInstance.IntersectionPoints.Add(intersectPoint);
                    }
                }
            }

            return (intersectionsInstance.IntersectionPoints.Count > 0) ? intersectionsInstance : null;
        }

        public static List<PipeAndMassIntersectionModel> GetIntersectionList(List<PipeModel> pipeModelList,
            List<MassFormModel> massFormModelList)
        {
            var pipeAndMassFormIntersectionList = new List<PipeAndMassIntersectionModel>();

            foreach (var massFormItem in massFormModelList)
            {
                foreach (var oPipe in pipeModelList)
                {

                    var pipeAndMassFormIntersection = GetIntersection(oPipe, massFormItem);
                    if (pipeAndMassFormIntersection == null)
                        continue;
                    pipeAndMassFormIntersectionList.Add(pipeAndMassFormIntersection);
                }
            }
            return pipeAndMassFormIntersectionList;
        }
    }
}
