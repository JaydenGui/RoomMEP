using Autodesk.Revit.UI;
using CutMEPCurvesByMassEdges.Models;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class MassFormRepo
    {
        public static List<MassFormModel> GetMassFormModels(ExternalCommandData commandData, string massFamilyName)
        {
            var massForms = FamilyInstanceUtils.GetAllTheFamilyInstancesOfFamily(commandData, massFamilyName);

            var massFormModels = new List<MassFormModel>();
            foreach (var massFormInstance in massForms)
            {
                var massFormModelItem = new MassFormModel
                {
                    Model = massFormInstance,
                    Faces = massFormInstance.GetFacesFromElement()
                };
                massFormModels.Add(massFormModelItem);
            }
            return massFormModels;
        }
    }
}
