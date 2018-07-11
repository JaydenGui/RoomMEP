using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitPSVUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AssignParamsRibbonUI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class RibbonUI : IExternalApplication
    {
        static AddInId appId = new AddInId(new Guid("4CF27338-FA4F-415F-A36C-2B6A154FB602"));
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication app)
        {
            //создаем новую вкладку на ленте
            var newRibbon = new RibbonUtils();
            string tabName = "SPGR Зонинг";
            newRibbon.CreateTab(app, tabName);
            #region Вкладка армирования проёма
            var appDesc1 = "Assign";
            var newPanel1 = newRibbon.AddPanel(app, tabName, appDesc1);
            var folderPath1 = @"C:\Program Files\BIMTech Utils\MEP\MEP Room";
            var appNamespace1 = "AssignParameterToInstancesInsideBBox";
            var dllPath1 = $@"Contents\{appNamespace1}.dll";
            var fullPath1 = Path.Combine(folderPath1, dllPath1);
            //добавляем кнопку на панель
            var button1 = newPanel1.AddButton(
                appDesc1,
                fullPath1,
                $"{appNamespace1}.Main");
            //задаём кнопке изображение
            var imageName1 = Path.Combine(folderPath1, @"UI\Images\mep.png");
            button1.Image = new BitmapImage(new Uri(imageName1, UriKind.Absolute));
            button1.LargeImage = new BitmapImage(new Uri(imageName1, UriKind.Absolute));
            button1.ToolTipImage = new BitmapImage(new Uri(imageName1, UriKind.Absolute));
            button1.ToolTip = appDesc1;

            var appDesc2 = "MEP Cut";
            var newPanel = newRibbon.AddPanel(app, tabName, appDesc2);
            var folderPath2 = @"C:\Program Files\BIMTech Utils\MEP\MEP Room";
            var appNamespace2 = "CutMEPCurvesByMassEdges";
            var dllPath2 = $@"Contents\{appNamespace2}.dll";
            var fullPath2 = Path.Combine(folderPath2, dllPath2);
            //добавляем кнопку на панель
            var button2 = newPanel.AddButton(
                appDesc2,
                fullPath2,
                $"{appNamespace2}.Main");
            //задаём кнопке изображение
            var imageName2 = Path.Combine(folderPath2, @"UI\Images\mep_cut.png");
            button2.Image = new BitmapImage(new Uri(imageName2, UriKind.Absolute));
            button2.LargeImage = new BitmapImage(new Uri(imageName2, UriKind.Absolute));
            button2.ToolTipImage = new BitmapImage(new Uri(imageName2, UriKind.Absolute));
            button2.ToolTip = appDesc2;
            #endregion
            return Result.Succeeded;
        }
    }
}
