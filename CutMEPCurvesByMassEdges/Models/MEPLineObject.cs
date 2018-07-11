using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Models
{
    public abstract class MEPLineObject
    {
        public Curve Curve { get; set; }
        public XYZ StarPoint { get; set; }
        public XYZ EndPoint { get; set; }
        public Connector ConnectorFirst { get; set; }
        public Connector ConnectorSecond { get; set; }
    }
}
