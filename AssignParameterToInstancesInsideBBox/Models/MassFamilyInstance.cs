using Autodesk.Revit.DB;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.ElementExts;

namespace AssignParameterToInstancesInsideBBox.Models
{
    public class MassFamilyInstance : BaseElement
    {
        public string Name { get; }
        public BoundingBoxXYZ BoundBox { get; }
        public Document Doc { get; set; }

        public MassFamilyInstance(FamilyInstance familyInstance)
        {
            Name = familyInstance.Name;
            BoundBox = familyInstance.get_BoundingBox(null);
            Element = familyInstance as Element;
            Doc = familyInstance.Document;
        }

        public List<List<XYZ>> GetFacePoints()
        {
            return Element.GetFacePointsFromElement();
        }
    }
}
