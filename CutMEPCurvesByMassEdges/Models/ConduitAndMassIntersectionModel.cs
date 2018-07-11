using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Models
{
    public class ConduitAndMassIntersectionModel : MEPLineAndMassIntersectionModel
    {
        public ConduitModel Conduit { get; set; }
    }
}
