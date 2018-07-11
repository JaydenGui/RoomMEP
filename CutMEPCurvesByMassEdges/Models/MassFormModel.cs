﻿using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitPSVUtils;
using Autodesk.Revit.UI;

namespace CutMEPCurvesByMassEdges.Models
{
    public class MassFormModel
    {
        public FamilyInstance Model { get; set; }
        public List<Face> Faces { get; set; }
    }
}
