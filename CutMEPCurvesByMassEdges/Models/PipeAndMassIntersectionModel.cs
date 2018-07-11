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
    public class PipeAndMassIntersectionModel : MEPLineAndMassIntersectionModel
    {
        public PipeModel Pipe { get; set; }
        
    }
}
