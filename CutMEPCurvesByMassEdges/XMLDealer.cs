using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static TextFilesDealer.DirectoryUtils;

namespace CutMEPCurvesByMassEdges
{
    public class XMLDealer
    {
        public void SaveUserInputToXML(string xmlFilePath,
            TextBox txtboxMassFormName)
        {
            var xmlDoc =
                new XElement("MEPRoom",
                        new XElement(nameof(txtboxMassFormName), txtboxMassFormName.Text)
                    );
            //если в директорию записывать нельзя, то уходим
            if (!IsDirectoryWritable(System.IO.Path.GetDirectoryName(xmlFilePath)))
            {
                MessageBox.Show($"Невозможно создать файл XML с настройками по пути {xmlFilePath}");
                return;
            }
            //Сохраняем файл
            xmlDoc.Save(xmlFilePath);
        }

        public void GetXMLDataToWPF(
            string xmlDataPath,
            ref TextBox txtboxMassFormName)
        {
            if (!File.Exists(xmlDataPath))
                return;
            var xmlDocUserInput = XDocument.Load(xmlDataPath);
            var xmlMainRebarSettings = xmlDocUserInput.Descendants("MEPRoom");

            txtboxMassFormName.Text = xmlMainRebarSettings.Descendants(nameof(txtboxMassFormName)).First().Value;
        }
    }
}
