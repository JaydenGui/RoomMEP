using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitPSVUtils;

namespace CutMEPCurvesByMassEdges.Models
{
    //Класс включает все фитинги и оборудование, которые относятся к MEP
    public class MEPElementModel
    {
        public FamilyInstance Model { get; set; }
        public List<Connector> Connectors { get; set; }
    }
}
