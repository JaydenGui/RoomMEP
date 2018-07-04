using AssignParameterToInstancesInsideBBox.Models;
using Autodesk.Revit.UI;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignParameterToInstancesInsideBBox.Repos
{
    public class MassFamilyRepo
    {
        public ExternalCommandData CommandData { get; }

        public MassFamilyRepo(ExternalCommandData commandData)
        {
            CommandData = commandData;
        }

        public List<MassFamilyInstance> GetInstancesByName(string familyName)
        {
            var massFInstanceList = new List<MassFamilyInstance>();
            var foundedFamilyInstance = FamilyInstanceUtils.GetAllTheFamilyInstancesOfFamily(CommandData, familyName);
            foreach (var fInstance in foundedFamilyInstance)
            {
                var massFInstance = new MassFamilyInstance(fInstance);
                massFInstanceList.Add(massFInstance);
            }
            return massFInstanceList;
        }
    }
}
