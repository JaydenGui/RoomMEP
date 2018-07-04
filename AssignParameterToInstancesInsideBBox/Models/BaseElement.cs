using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignParameterToInstancesInsideBBox.Models
{
    public class BaseElement
    {
        //Уникальные номер экземпляра
        private string _uid;

        public string UID
        {
            get { return _uid; }
            set { _uid = value; }
        }

        public Element Element { get; set; }
    }
}
