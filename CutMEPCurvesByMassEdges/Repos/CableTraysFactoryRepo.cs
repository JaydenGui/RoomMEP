using Autodesk.Revit.DB;
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
    public class CableTraysFactoryRepo
    {
        public List<CableTrayModel> CableTrays { get; set; } = new List<CableTrayModel>();

        public void CreateCableTraysFromIntersectionPoints(
            List<CableTrayAndMassIntersectionModel> cableTrayAndMassFormIntersectionList,
            ExternalCommandData commandData)
        {

            foreach (var cableTrayAndMassInt in cableTrayAndMassFormIntersectionList)
            {
                //Выбираем точки, по которым будем создавать трубы
                var pointsToCreateCableTrays = new List<XYZ>();
                pointsToCreateCableTrays.Add(cableTrayAndMassInt.CableTray.StarPoint);
                pointsToCreateCableTrays.AddRange(cableTrayAndMassInt.IntersectionPoints
                                                .OrderBy(p => p.DistanceTo(cableTrayAndMassInt.CableTray.StarPoint)).ToList());
                pointsToCreateCableTrays.Add(cableTrayAndMassInt.CableTray.EndPoint);
                bool isNewCableTrayCreated = false;
                for (int i = 0; i < pointsToCreateCableTrays.Count; i++)
                {
                    if (i == 0)
                        continue;
                    var currentPoint = pointsToCreateCableTrays[i];
                    var prevPoint = pointsToCreateCableTrays[i - 1];
                    if (currentPoint.IsEqualByXYZ(prevPoint, 5))
                        continue;
                    //Создаём новую трубу по новым точкам, но присваивая свойства прошлой трубы
                    var newCableTray = CableTraysUtils.CreateNewCableTrayByTypeOfExisted(
                        cableTrayAndMassInt.CableTray.Model, currentPoint, prevPoint, commandData);
                    if (newCableTray == null)
                        continue;
                    var newCableTrayModel = CableTrayModel.Create(newCableTray);
                    if (newCableTrayModel != null)
                    {
                        CableTrays.Add(newCableTrayModel);
                        isNewCableTrayCreated = true;
                    }
                }
                //если создавался новый элемент на основе старого, то старый удаляем.
                if (isNewCableTrayCreated)
                    DeleteUtils.DeleteElements(commandData, new List<Element> { cableTrayAndMassInt.CableTray.Model });
            }
        }

        public static void ConnectCableTraysAndMEPElementsWithConnectorsInSameLocation(List<CableTrayModel> cableTrayModels,
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
                foreach (var cableTrayModelItem in cableTrayModels)
                {
                    foreach (var mepElementModelItem in mepElementModels)
                    {
                        foreach (var mepConnector in mepElementModelItem.Connectors)
                        {
                            if (cableTrayModelItem.ConnectorFirst.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!cableTrayModelItem.ConnectorFirst.IsConnectedTo(mepConnector))
                                {
                                    cableTrayModelItem.ConnectorFirst.ConnectTo(mepConnector);
                                }
                            }

                            if (cableTrayModelItem.ConnectorSecond.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!cableTrayModelItem.ConnectorSecond.IsConnectedTo(mepConnector))
                                {
                                    cableTrayModelItem.ConnectorSecond.ConnectTo(mepConnector);
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
