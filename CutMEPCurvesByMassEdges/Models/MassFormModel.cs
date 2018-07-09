using Autodesk.Revit.DB;
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
