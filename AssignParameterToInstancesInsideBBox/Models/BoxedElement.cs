using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignParameterToInstancesInsideBBox.Models
{
    public class BoxedElement : BaseElement
    {
        public Element MassElement { get; }
        public BuiltInCategory BuiltInCategory { get; }

        public BoxedElement(Element massElement, Element element)
        {
            MassElement = massElement;
            Element = element as Element;
        }
    }
}
