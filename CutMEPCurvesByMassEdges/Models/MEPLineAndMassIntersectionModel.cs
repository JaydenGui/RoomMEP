using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Models
{
    public abstract class MEPLineAndMassIntersectionModel
    {
        public MassFormModel MassForm { get; set; }
        public List<XYZ> IntersectionPoints { get; set; } = new List<XYZ>();
    }
}
