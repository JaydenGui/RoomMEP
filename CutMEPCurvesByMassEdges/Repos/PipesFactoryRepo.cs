using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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
    public class PipesFactoryRepo
    {
        public List<PipeModel> Pipes { get; set; } = new List<PipeModel>();

        public void CreatePipesFromIntersectionPoints(
            List<PipeAndMassIntersectionModel> pipeAndMassFormIntersectionList,
            ExternalCommandData commandData)
        {

            #region Создаём трубы из точек пересечения
            foreach (var pipeAndMassInt in pipeAndMassFormIntersectionList)
            {
                //Выбираем точки, по которым будем создавать трубы
                var pointsToCreatePipes = new List<XYZ>();
                pointsToCreatePipes.Add(pipeAndMassInt.Pipe.StarPoint);
                pointsToCreatePipes.AddRange(pipeAndMassInt.IntersectionPoints
                                                .OrderBy(p => p.DistanceTo(pipeAndMassInt.Pipe.StarPoint)).ToList());
                pointsToCreatePipes.Add(pipeAndMassInt.Pipe.EndPoint);
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
                    var newPipe = PipesUtils.CreateNewPipeByTypeOfExisted(
                        pipeAndMassInt.Pipe.Model, currentPoint, prevPoint, commandData);
                    if (newPipe == null)
                        continue;
                    var newPipeModel = PipeModel.Create(newPipe);
                    if (newPipeModel != null)
                    {
                        Pipes.Add(newPipeModel);
                        isNewPipeCreated = true;
                    }
                }
                //если создавался новый элемент на основе старого, то старый удаляем.
                if (isNewPipeCreated)
                    DeleteUtils.DeleteElements(commandData, new List<Element> { pipeAndMassInt.Pipe.Model });
            }
            #endregion

            #region Соединяем новые трубы, соединения которых разорваны
            foreach (var oPipeFirst in Pipes)
            {
                foreach (var oPipeSecond in Pipes)
                {
                    oPipeFirst.Model.ConnectToWithUnionFitting(oPipeSecond.Model, commandData);
                }
            }
            #endregion
        }

        public static void ConnectPipesAndMEPElementsWithConnectorsInSameLocation(List<PipeModel> pipeModels,
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
                foreach (var pipeModelItem in pipeModels)
                {
                    foreach (var mepElementModelItem in mepElementModels)
                    {
                        foreach (var mepConnector in mepElementModelItem.Connectors)
                        {
                            if (pipeModelItem.ConnectorFirst.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!pipeModelItem.ConnectorFirst.IsConnectedTo(mepConnector))
                                {
                                    pipeModelItem.ConnectorFirst.ConnectTo(mepConnector);
                                }
                            }

                            if (pipeModelItem.ConnectorSecond.Origin.IsEqualByXYZ(mepConnector.Origin, 5))
                            {
                                if (!pipeModelItem.ConnectorSecond.IsConnectedTo(mepConnector))
                                {
                                    pipeModelItem.ConnectorSecond.ConnectTo(mepConnector);
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
