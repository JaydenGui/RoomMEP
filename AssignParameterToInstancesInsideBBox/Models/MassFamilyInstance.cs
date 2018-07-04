using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignParameterToInstancesInsideBBox.Models
{
    public class MassFamilyInstance : BaseElement
    {
        public string Name { get; }
        public BoundingBoxXYZ BoundBox { get; }

        public MassFamilyInstance(FamilyInstance familyInstance)
        {
            Name = familyInstance.Name;
            BoundBox = familyInstance.get_BoundingBox(null);
            Element = familyInstance as Element;
        }
    }
}
