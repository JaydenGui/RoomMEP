using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitPSVUtils;
using static RevitPSVUtils.PipeExts;

namespace CutMEPCurvesByMassEdges.Models
{
    public class PipeAndMassFormIntersection
    {
        public PipeModel Pipe { get; set; }
        public MassFormModel MassForm { get; set; }
        public List<XYZ> IntersectionPoints { get; set; } = new List<XYZ>();

        private static PipeAndMassFormIntersection GetIntersection(PipeModel pipeModel, MassFormModel massFormModel)
        {
            var intersectionsInstance = new PipeAndMassFormIntersection { Pipe = pipeModel, MassForm = massFormModel };
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
                    //проверяем находится ли точка на линии
                    if (GeomShark.PointUtils.IsPointBetweenOtherTwoPoints(
                                                            pipeCurveStartPoint.X, pipeCurveStartPoint.Y,
                                                            pipeCurveEndPoint.X, pipeCurveEndPoint.Y,
                                                            intersectPoint.X, intersectPoint.Y, 4))
                    {
                        intersectionsInstance.IntersectionPoints.Add(intersectPoint);
                    }
                }
            }

            return (intersectionsInstance.IntersectionPoints.Count > 0) ? intersectionsInstance : null;
        }

        public static List<PipeAndMassFormIntersection> GetIntersectionList(List<PipeModel> pipeModelList,
            List<MassFormModel> massFormModelList)
        {
            var pipeAndMassFormIntersectionList = new List<PipeAndMassFormIntersection>();

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
