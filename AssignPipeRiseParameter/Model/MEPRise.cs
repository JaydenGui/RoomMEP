using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitPSVUtils;

namespace AssignPipeRiseParameter.Model
{
    public class MEPRise
    {
        public Element Model { get; set; }

        public List<MEPRise> CreateMEPLines(List<Element> mepCurves, double heightMaxMM)
        {
            var mepRises = new List<MEPRise>();
            foreach (var mepCurveElem in mepCurves)
            {
                var mepCurve = mepCurveElem as MEPCurve;
                if (mepCurve == null)
                    continue;
                var points = mepCurve.GetStartAndEndPoint();
                if (points.Count < 2)
                    continue;
                if (points[0] == null || points[1] == null)
                    continue;
                if (Math.Abs(points[0].Z - points[1].Z) < heightMaxMM)
                    continue;

                mepRises.Add(new MEPRise
                {
                    Model = mepCurve
                });
            }
            return mepRises;
        }
    }
}
