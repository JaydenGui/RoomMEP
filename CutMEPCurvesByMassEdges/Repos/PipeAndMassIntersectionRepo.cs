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
        private static PipeAndMassIntersectionModel GetIntersection(
            PipeModel pipeModel,
            MassFormModel massFormModel,
            List<XYZ> fittingPointList)
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
                    if (intersectPoint.IsEqualByXYZ(pipeCurveStartPoint, 5) ||
                        intersectPoint.IsEqualByXYZ(pipeCurveEndPoint, 5))
                        continue;

                    //проверяем находится ли точка на линии по ХУ и по высотной отметке Z
                    bool isIntersectPointInRange
                        = NumberUtils.IsInRange(
                            intersectPoint.Z,
                            Math.Min(pipeCurveStartPoint.Z, pipeCurveEndPoint.Z),
                            Math.Max(pipeCurveStartPoint.Z, pipeCurveEndPoint.Z));

                    if (intersectPoint.IsPointBetweenOtherPoints(pipeCurveEndPoint, pipeCurveStartPoint, 5) &&
                         isIntersectPointInRange)
                    {
                        intersectionsInstance.IntersectionPoints.Add(intersectPoint);
                    }
                }
            }

            return (intersectionsInstance.IntersectionPoints.Count > 0) ? intersectionsInstance : null;
        }

        public static List<PipeAndMassIntersectionModel> GetIntersectionList(List<PipeModel> pipeModelList,
            List<MassFormModel> massFormModelList, List<XYZ> fittingPointList)
        {
            var pipeAndMassFormIntersectionList = new List<PipeAndMassIntersectionModel>();

            foreach (var massFormItem in massFormModelList)
            {
                foreach (var oPipe in pipeModelList)
                {

                    var pipeAndMassFormIntersection = GetIntersection(oPipe, massFormItem, fittingPointList);
                    if (pipeAndMassFormIntersection == null)
                        continue;
                    pipeAndMassFormIntersectionList.Add(pipeAndMassFormIntersection);
                }
            }
            return pipeAndMassFormIntersectionList;
        }
    }
}
