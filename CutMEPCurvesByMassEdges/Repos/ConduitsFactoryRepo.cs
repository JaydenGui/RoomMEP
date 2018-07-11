using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CutMEPCurvesByMassEdges.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitPSVUtils;

namespace CutMEPCurvesByMassEdges.Repos
{
    public class ConduitsFactoryRepo
    {
        public List<ConduitModel> Conduits { get; set; } = new List<ConduitModel>();

        public void CreateConduitsFromIntersectionPoints(
            List<ConduitAndMassIntersectionModel> conduitAndMassFormIntersectionList,
            ExternalCommandData commandData)
        {

            foreach (var conduitAndMassInt in conduitAndMassFormIntersectionList)
            {
                //Выбираем точки, по которым будем создавать трубы
                var pointsToCreateConduits = new List<XYZ>();
                pointsToCreateConduits.Add(conduitAndMassInt.Conduit.StarPoint);
                pointsToCreateConduits.AddRange(conduitAndMassInt.IntersectionPoints
                                                .OrderBy(p => p.DistanceTo(conduitAndMassInt.Conduit.StarPoint)).ToList());
                pointsToCreateConduits.Add(conduitAndMassInt.Conduit.EndPoint);

                bool isNewConduitCreated = false;
                for (int i = 0; i < pointsToCreateConduits.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var currentPoint = pointsToCreateConduits[i];
                    var prevPoint = pointsToCreateConduits[i - 1];
                    if (currentPoint.IsEqualByXYZ(prevPoint, 5))
                        continue;
                    //Создаём новую трубу по новым точкам, но присваивая свойства прошлой трубы
                    var newConduit = ConduitsUtils.CreateNewConduitByTypeOfExisted(
                        conduitAndMassInt.Conduit.Model, currentPoint, prevPoint, commandData);
                    if (newConduit == null)
                        continue;
                    var newConduitModel = ConduitModel.Create(newConduit);
                    if (newConduitModel != null)
                    {
                        Conduits.Add(newConduitModel);
                        isNewConduitCreated = true;
                    }
                }

                if (isNewConduitCreated)
                    DeleteUtils.DeleteElements(commandData, new List<Element> { conduitAndMassInt.Conduit.Model });
            }
        }

        public static void ConnectConduitsAndMEPElementsWithConnectorsInSameLocation(List<ConduitModel> conduitModels,
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
                foreach (var conduitModelItem in conduitModels)
                {
                    foreach (var mepElementModelItem in mepElementModels)
                    {
                        foreach (var mepConnector in mepElementModelItem.Connectors)
                        {
                            if (conduitModelItem.ConnectorFirst.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!conduitModelItem.ConnectorFirst.IsConnectedTo(mepConnector))
                                {
                                    conduitModelItem.ConnectorFirst.ConnectTo(mepConnector);
                                }
                            }

                            if (conduitModelItem.ConnectorSecond.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!conduitModelItem.ConnectorSecond.IsConnectedTo(mepConnector))
                                {
                                    conduitModelItem.ConnectorSecond.ConnectTo(mepConnector);
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
