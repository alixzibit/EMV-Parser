using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
namespace EMV_Parser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NanoPersoXMLParser : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isDataGridEmpty = true;

        public bool IsDataGridEmpty
        {
            get => _isDataGridEmpty;
            set
            {
                _isDataGridEmpty = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TagData> TagDataList { get; set; } = new ObservableCollection<TagData>();

        public NanoPersoXMLParser()
        {
            InitializeComponent();
            NanoPersoDataGrid.ItemsSource = TagDataList;
            DataContext = this;
        }
        private string GetAttributeValueBeforePath(XElement node, string attributeName)
        {
            var attributes = node.Attributes().ToList();
            int pathIndex = attributes.FindIndex(a => a.Name == "PATH");

            if (pathIndex == -1) return null;

            int valueIndex = attributes.FindIndex(a => a.Name == attributeName);

            if (valueIndex == -1 || valueIndex >= pathIndex) return null;

            return attributes[valueIndex].Value;
        }
        private void ClearTable(object sender, RoutedEventArgs e)
        {
            TagDataList.Clear();
            IsDataGridEmpty = true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void LoadTagsFromNanoPersoButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "NanoPerso XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                XElement rootElement = XElement.Load(openFileDialog.FileName);

                var nanoPersoParserWindow = this;

                foreach (var node in rootElement.Descendants("NODE"))
                {
                    string nodeName = node.Attribute("NAME")?.Value;
                    string tag = node.Attribute("TAG")?.Value;
                    string valueBeforePath = GetAttributeValueBeforePath(node, "VALUE");

                    XElement parentNode = node;
                    while (parentNode.Parent != null && parentNode.Parent.Attribute("PATH") != null)
                    {
                        parentNode = parentNode.Parent;
                    }
                    string parentPath = parentNode.Attribute("PATH")?.Value;
                    string dgi = parentPath.StartsWith("DT") ? parentPath.Substring(3) : parentPath;

                    if (!string.IsNullOrEmpty(nodeName) && !string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(valueBeforePath))
                    {
                        var tagData = new TagData
                        {
                            ElementName = nodeName,
                            Tag = tag,
                            Value = valueBeforePath,
                            Category = $"{nodeName} ({dgi})"
                        };

                        //  not implemented: ParsedValue property
                        // if we want to convert the Value to hexadecimal:
                        // tagData.ParsedValue = Convert.ToInt32(tagData.Value).ToString("X");

                        nanoPersoParserWindow.TagDataList.Add(tagData);
                    }
                }

                // Update the IsDataGridEmpty property after loading the tags
                IsDataGridEmpty = TagDataList.Count == 0;
            }
        }




        private void FillFromVPACPVButtonClick(object sender, RoutedEventArgs e)
        {
            var mainDataGridItems = ((MainWindow)Application.Current.MainWindow).TagDataGrid.Items;

            if (mainDataGridItems.Count == 0)
            {
                MessageBox.Show("Please parse VPA/CPV data in the main window first.", "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MatchAdditionalInfo();
        }
        private void GenerateReport(List<TagData> matchedItems, List<TagData> unmatchedItems)
        {
            StringBuilder reportBuilder = new StringBuilder();

            reportBuilder.AppendLine("Matched Items:");
            foreach (TagData matchedItem in matchedItems)
            {
                reportBuilder.AppendLine(matchedItem.ToString());
            }

            reportBuilder.AppendLine();
            reportBuilder.AppendLine("Unmatched Items:");
            foreach (TagData unmatchedItem in unmatchedItems)
            {
                reportBuilder.AppendLine(unmatchedItem.ToString());
            }

            // Display the report in a MessageBox
            MessageBox.Show(reportBuilder.ToString(), "Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MatchAdditionalInfo()
        {
            var mainDataGridItems = ((MainWindow)Application.Current.MainWindow).TagDataGrid.Items;
            var nanoPersoDataGridItems = TagDataList;

            List<TagData> unmatchedItems = new List<TagData>();
            List<TagData> matchedItems = new List<TagData>();

            foreach (var mainDataGridItem in mainDataGridItems)
            {
                bool isMatched = false;
                TagData mainElement = mainDataGridItem as TagData;

                if (mainElement == null)
                    continue;

                // Extract the actual tag from the data grid Tag field
                string actualTag = mainElement.Tag.Split(' ')[0];

                // Trim the ElementName from MainWindow
                string[] mainElementNameParts = mainElement.ElementName.Split('-');
                string mainElementNameTrimmed = mainElementNameParts.Length > 1 ? mainElementNameParts[1].Trim() : mainElementNameParts[0].Trim();

                foreach (TagData nanoPersoDataGridItem in nanoPersoDataGridItems)
                {
                    string[] nanoPersoElementNameParts = nanoPersoDataGridItem.ElementName.Split('-');
                    string nanoPersoElementNameTrimmed = nanoPersoElementNameParts.Length > 1 ? nanoPersoElementNameParts[1].Trim() : nanoPersoElementNameParts[0].Trim();

                    bool isPartialMatch = actualTag == nanoPersoDataGridItem.Tag &&
                                          mainElementNameTrimmed.Contains(nanoPersoElementNameTrimmed);

                    if (isPartialMatch)
                    {
                        // Update the nanoPersoDataGridItem with the matched information
                        nanoPersoDataGridItem.ParsedValue = mainElement.Value;
                        matchedItems.Add(mainElement);
                        isMatched = true;
                        break;
                    }
                }

                if (!isMatched)
                {
                    unmatchedItems.Add(mainElement);
                }
            }

            // Generate simple report - need to work on saving or adding logging to code to record this output
            GenerateReport(matchedItems, unmatchedItems);
        }



        private void SaveEditsClick(object sender, RoutedEventArgs e)
        {
            // Saving xml file with new parsed values - check logic from exiting xml code from OMADP
            MessageBox.Show("Saving edits to file is currently under testing.");
        }

    }
}




