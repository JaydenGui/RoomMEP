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

namespace AssignParameterToInstancesInsideBBox
{
    public class XMLDealer
    {
        public void SaveUserInputToXML(string xmlFilePath, 
            TextBox txtbox_massFamilyName, TextBox txtbox_massParameterName,
            TextBox txtbox_instancesInsideMassParameterName)
        {
            var xmlDoc =
                new XElement("MEPRoom",
                        new XElement(nameof(txtbox_massFamilyName), txtbox_massFamilyName.Text),
                        new XElement(nameof(txtbox_massParameterName), txtbox_massParameterName.Text),
                        new XElement(nameof(txtbox_instancesInsideMassParameterName), txtbox_instancesInsideMassParameterName.Text)
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
            ref TextBox txtbox_massFamilyName, 
            ref TextBox txtbox_massParameterName, 
            ref TextBox txtbox_instancesInsideMassParameterName)
        {
            if (!File.Exists(xmlDataPath))
                return;
            var xmlDocUserInput = XDocument.Load(xmlDataPath);
            var xmlMainRebarSettings = xmlDocUserInput.Descendants("MEPRoom");

            txtbox_massFamilyName.Text = xmlMainRebarSettings.Descendants(nameof(txtbox_massFamilyName)).First().Value;
            txtbox_massParameterName.Text = xmlMainRebarSettings.Descendants(nameof(txtbox_massParameterName)).First().Value;
            txtbox_instancesInsideMassParameterName.Text = xmlMainRebarSettings
                                                            .Descendants(nameof(txtbox_instancesInsideMassParameterName)).First().Value;
            
        }
    }
}
