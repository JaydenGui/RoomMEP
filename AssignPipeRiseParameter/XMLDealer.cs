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
        //txtboxParameterRise, 
        //                                txtboxRiseToleranceByXY, txtboxRiseToleranceByZ
        public void SaveUserInputToXML(string xmlFilePath,
            TextBox txtboxParameterRise, TextBox txtboxRiseToleranceByXY, TextBox txtboxRiseToleranceByZ)
        {
            var xmlDoc =
                new XElement("AssignPipeRiseParameter",
                        new XElement(nameof(txtboxParameterRise), txtboxParameterRise.Text),
                        new XElement(nameof(txtboxRiseToleranceByXY), txtboxRiseToleranceByXY.Text),
                        new XElement(nameof(txtboxRiseToleranceByZ), txtboxRiseToleranceByZ.Text)

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
            ref TextBox txtboxParameterRise, ref TextBox txtboxRiseToleranceByXY, ref TextBox txtboxRiseToleranceByZ)
        {
            if (!File.Exists(xmlDataPath))
                return;
            var xmlDocUserInput = XDocument.Load(xmlDataPath);
            var xmlMainRebarSettings = xmlDocUserInput.Descendants("AssignPipeRiseParameter");

            if (xmlMainRebarSettings.Descendants(nameof(txtboxParameterRise)).FirstOrDefault() != null)
                txtboxParameterRise.Text = xmlMainRebarSettings.Descendants(nameof(txtboxParameterRise)).FirstOrDefault().Value;
            if (xmlMainRebarSettings.Descendants(nameof(txtboxRiseToleranceByXY)).FirstOrDefault() != null)
                txtboxRiseToleranceByXY.Text = xmlMainRebarSettings.Descendants(nameof(txtboxRiseToleranceByXY)).FirstOrDefault().Value;
            if (xmlMainRebarSettings.Descendants(nameof(txtboxRiseToleranceByZ)).FirstOrDefault() != null)
                txtboxRiseToleranceByZ.Text = xmlMainRebarSettings.Descendants(nameof(txtboxRiseToleranceByZ)).FirstOrDefault().Value;
        }
    }
}
