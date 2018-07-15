using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CutMEPCurvesByMassEdges.Models;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitPSVUtils.DuctExts;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class DuctsFactoryRepo
    {
        public List<DuctModel> Ducts { get; set; } = new List<DuctModel>();

        public void CreateDuctsFromIntersectionPoints(
            List<DuctAndMassIntersectionModel> ductAndMassFormIntersectionList,
            ExternalCommandData commandData)
        {

            foreach (var ductAndMassInt in ductAndMassFormIntersectionList)
            {
                //Выбираем точки, по которым будем создавать трубы
                var pointsToCreatePipes = new List<XYZ>();
                pointsToCreatePipes.Add(ductAndMassInt.Duct.StarPoint);
                pointsToCreatePipes.AddRange(ductAndMassInt.IntersectionPoints
                                                .OrderBy(p => p.DistanceTo(ductAndMassInt.Duct.StarPoint)).ToList());
                pointsToCreatePipes.Add(ductAndMassInt.Duct.EndPoint);

                bool isNewPipeCreated = false;
                for (int i = 0; i < pointsToCreatePipes.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var currentPoint = pointsToCreatePipes[i];
                    var prevPoint = pointsToCreatePipes[i - 1];
                    if (currentPoint.IsEqualByXYZ(prevPoint, 5))
                        continue;
                    //Создаём новую трубу по новым точкам, но присваивая свойства прошлой трубы
                    var newDuct = DuctsUtils.CreateNewDuctByTypeOfExisted(
                        ductAndMassInt.Duct.Model, currentPoint, prevPoint, commandData);
                    if (newDuct == null)
                        continue;
                    var newDuctModel = DuctModel.Create(newDuct);
                    if (newDuctModel != null)
                    {
                        isNewPipeCreated = true;
                        Ducts.Add(newDuctModel);
                    }

                }

                foreach (var oDuctFirst in Ducts)
                {
                    foreach (var oDuctSecond in Ducts)
                    {
                        oDuctFirst.Model.ConnectToWithUnionFitting(oDuctSecond.Model, commandData);
                    }
                }

                if (isNewPipeCreated)
                    DeleteUtils.DeleteElements(commandData, new List<Element> { ductAndMassInt.Duct.Model });
            }
        }

        public static void ConnectPipesAndMEPElementsWithConnectorsInSameLocation(List<DuctModel> DuctModels,
            List<MEPElementModel> mepElementModels, ExternalCommandData commandData)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            //Проверяем совпадают ли точки коннекторов труб и mep элементов
            //и если совпадают - соединяем коннекторы
            using (var t = new Autodesk.Revit.DB.Transaction(doc, "Deal with connectors"))
            {
                t.Start();
                foreach (var DuctModelItem in DuctModels)
                {
                    foreach (var mepElementModelItem in mepElementModels)
                    {
                        foreach (var mepConnector in mepElementModelItem.Connectors)
                        {
                            if (DuctModelItem.ConnectorFirst.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!DuctModelItem.ConnectorFirst.IsConnectedTo(mepConnector))
                                {
                                    DuctModelItem.ConnectorFirst.ConnectTo(mepConnector);
                                }
                            }

                            if (DuctModelItem.ConnectorSecond.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!DuctModelItem.ConnectorSecond.IsConnectedTo(mepConnector))
                                {
                                    DuctModelItem.ConnectorSecond.ConnectTo(mepConnector);
                                }
                            }
                        }
                    }
                }
                t.Commit();
            }
        }
    }
}
