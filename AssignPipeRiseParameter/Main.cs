using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Creation;
using System.Text.RegularExpressions;

namespace AssignPipeRiseParameter
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("F0DDF653-5E93-429E-880E-67EE16EA556F"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var form = new MainForm(commandData);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
