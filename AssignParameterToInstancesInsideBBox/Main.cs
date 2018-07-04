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

namespace AssignParameterToInstancesInsideBBox
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        static AddInId appId = new AddInId(new Guid("727310C9-3774-491A-A7C1-7EBD21F1EDA1"));
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
