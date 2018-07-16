using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Models
{
    public class FittingModel
    {
        public List<XYZ> ConnectorPoints { get; set; } = new List<XYZ>();
        public FamilyInstance Model { get; set; }

        
    }
}
