using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using OfficeOpenXml;


namespace EMV_Parser
{
    public partial class MainWindow : Window
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private List<TagData> tagDataList;

        public MainWindow()
        {
            InitializeComponent();

            tagDataList = new List<TagData>();
            TagDataGrid.ItemsSource = tagDataList;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void InputFileBrowseButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "VPA HTML File (*.html)|*.html|MCHIP CPV XML File (*.xml)|*.xml|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = openFileDialog.FileName;
            }
        }
        //commented as CSV format is not being used in final code
        //private void OutputFileBrowseButtonClick(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    saveFileDialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
        //    if (saveFileDialog.ShowDialog() == true)
        //    {
        //        OutputFileTextBox.Text = saveFileDialog.FileName;
        //    }
        //}

        private void EMVTagsDictionaryButtonClick(object sender, RoutedEventArgs e)
        {
            NanoPersoXMLParser emvTagsDictionaryWindow = new NanoPersoXMLParser();
            emvTagsDictionaryWindow.Show();
        }

        private void ParseButtonClick(object sender, RoutedEventArgs e)
        {
            string filePath = InputFileTextBox.Text;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File does not exist.");
                return;
            }

            try
            {
                List<TagData> tagDataList = null;
                string fileExtension = Path.GetExtension(filePath);

                if (fileExtension == ".xml")
                {
                    XmlParser parser = new XmlParser(filePath);
                    tagDataList = parser.Parse();
                }
                else if (fileExtension == ".html" || fileExtension == ".htm")
                {
                    HtmlParser parser = new HtmlParser(filePath);
                    tagDataList = parser.Parse();
      
                }

                else
                {
                    MessageBox.Show("Invalid file format. Please select an HTML or XML file.");
                    return;
                }

                TagDataGrid.ItemsSource = tagDataList;

                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                logger.Info($"Successfully parsed {filePath}. Tag count: {tagDataList.Count}");

                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"[{DateTime.Now.ToString()}] Successfully parsed {filePath}. Tag count: {tagDataList.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception occurred while parsing file {filePath}. Exception details: {ex.Message}");
                logger.Error(ex, $"Exception occurred while parsing file {filePath}. Exception details: {ex.Message}");

                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"[{DateTime.Now.ToString()}] Exception occurred while parsing file {filePath}. Exception details: {ex.Message}");
                }
            }
        }
    }
}