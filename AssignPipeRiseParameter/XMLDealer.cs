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

namespace AssignPipeRiseParameter
{
    public class XMLDealer
    {
        public void SaveUserInputToXML(string xmlFilePath,
            TextBox txtboxParameterRise, TextBox txtboxParamLength)
        {
            var xmlDoc =
                new XElement("AssignPipeRiseParameter",
                        new XElement(nameof(txtboxParameterRise), txtboxParameterRise.Text),
                        new XElement(nameof(txtboxParamLength), txtboxParamLength.Text)

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
            ref TextBox txtboxParameterRise, ref TextBox txtboxParamLength)
        {
            if (!File.Exists(xmlDataPath))
                return;
            var xmlDocUserInput = XDocument.Load(xmlDataPath);
            var xmlMainRebarSettings = xmlDocUserInput.Descendants("AssignPipeRiseParameter");

            txtboxParameterRise.Text = xmlMainRebarSettings.Descendants(nameof(txtboxParameterRise)).First().Value;
            txtboxParamLength.Text = xmlMainRebarSettings.Descendants(nameof(txtboxParamLength)).First().Value;
        }
    }
}
