using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
//This not being used, this xaml was previously used for testing and viewing data bound to a treeview instead of datagrid view
//like in NanoPersoXMLParser - please view NanoPersoXMLParser instead if working on current application implementation
namespace EMV_Parser
{
    public partial class EMVTagsDictionaryWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<CardType> _cardTypes = new ObservableCollection<CardType>();
        public ObservableCollection<CardType> CardTypes
        {
            get { return _cardTypes; }
            set
            {
                _cardTypes = value;
                OnPropertyChanged(nameof(CardTypes));
            }
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
        }
        public EMVTagsDictionaryWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        //// ... ParseNanoPersoXml method remains the same ...
        //private ObservableCollection<CardType> ParseNanoPersoXml(string inputFilePath)
        //{
        //    XDocument xdoc = XDocument.Load(inputFilePath);
        //    XElement treeViewRoot = xdoc.Element("TreeView");

        //    var cardTypes = from cardTypeNode in treeViewRoot.Elements("NODE")
        //                    where cardTypeNode.Attribute("TYPE").Value == "DGI_EMV" || cardTypeNode.Attribute("TYPE").Value == "DGI_PSE" || cardTypeNode.Attribute("TYPE").Value == "DGI_PPSE"
        //                    group cardTypeNode by cardTypeNode.Attribute("NAME").Value into cardTypeGroup
        //                    select new CardType
        //                    {
        //                        TypeName = cardTypeGroup.Key,
        //                        DataGroups = new ObservableCollection<DataGroup>(
        //                            from dataGroupNode in cardTypeGroup.Elements("NODE")
        //                            select new DataGroup
        //                            {
        //                                GroupName = dataGroupNode.Attribute("NAME").Value,
        //                                DataElements = new ObservableCollection<DataElement>(
        //                                    from dataElementNode in dataGroupNode.Elements("NODE")
        //                                    select new DataElement
        //                                    {
        //                                        ElementName = dataElementNode.Attribute("NAME").Value,
        //                                        DataSubElements = new ObservableCollection<DataSubElement>(
        //                                            from dataSubElementNode in dataElementNode.Elements("NODE")
        //                                            select new DataSubElement
        //                                            {
        //                                                Name = dataSubElementNode.Attribute("NAME").Value,
        //                                                Value = dataSubElementNode.Attribute("VALUE").Value
        //                                            })
        //                                    })
        //                            })
        //                    };

        //    return new ObservableCollection<CardType>(cardTypes);
        //}


        //private void AddTagButtonClick(object sender, RoutedEventArgs e)
        //{
        //    // Implement logic to add a new tag manually.
        //    // Need to check code from OMADP.
        //}

        private void LoadTagsFromNanoPersoButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "NanoPerso XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                XElement rootElement = XElement.Load(openFileDialog.FileName);

                var cardType = new CardType { TypeName = "Filtered EMV Tags" };

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
                        var dataGroup = cardType.DataGroups.FirstOrDefault(g => g.GroupName == $"{nodeName} ({dgi})");

                        if (dataGroup == null)
                        {
                            dataGroup = new DataGroup() { GroupName = $"{nodeName} ({dgi})" };
                            cardType.DataGroups.Add(dataGroup);
                        }

                        var dataSubElement = new DataSubElement
                        {
                            SubElementName = nodeName,
                            Tag = tag,
                            Value = valueBeforePath
                        };

                        dataGroup.DataSubElements.Add(dataSubElement);
                    }
                }

                CardTypes.Add(cardType);
            }
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

        private void MatchAdditionalInfo()
        {
            var mainDataGridItems = ((MainWindow)Application.Current.MainWindow).TagDataGrid.Items;
            var emvTagsTreeViewItems = EMVTagsTreeView.Items;

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

                foreach (CardType cardType in emvTagsTreeViewItems)
                {
                    foreach (DataGroup dataGroup in cardType.DataGroups)
                    {
                        foreach (DataSubElement subElement in dataGroup.DataSubElements)
                        {
                            // Update the matching condition with the actual tag
                            bool isPartialMatch = actualTag == subElement.Tag &&
                                                  mainElement.ElementName.Contains(subElement.SubElementName);

                            // Check the conditions for qVSDC
                            bool isQVSDC = mainElement.Category.Contains("qVSDC") && subElement.SubElementName.Contains("qVSDC");

                            // Check the conditions for VSDC & qVSDC
                            bool isVSDCAndQVSDC = mainElement.Category.Contains("VSDC & qVSDC");

                            // Check the conditions for Application Interchange Profile
                            bool isAppInterchangeProfile9115 = mainElement.ElementName.Contains("Application Interchange Profile") &&
                                                              mainElement.Tag == "82" &&
                                                              subElement.SubElementName.Contains("9115");

                            bool isAppInterchangeProfile9117 = mainElement.ElementName.Contains("Application Interchange Profile") &&
                                                              mainElement.Tag == "82" &&
                                                              subElement.SubElementName.Contains("9117");

                            // Check the conditions for readcontactless
                            bool isReadContactless = mainElement.Category.Contains("readcontactless") &&
                                                     subElement.SubElementName.Contains("PayPass");

                            if (isPartialMatch && (isQVSDC || isVSDCAndQVSDC || isAppInterchangeProfile9115 || isAppInterchangeProfile9117 || isReadContactless))
                            {
                                // Add additional text block with new value
                                subElement.AdditionalInfo = new AdditionalInfo { Label = mainElement.Category, Value = mainElement.Value };
                                matchedItems.Add(mainElement);
                                isMatched = true;
                                break;
                            }
                        }
                        if (isMatched) break;
                    }

                    if (isMatched) break;
                }

                if (isMatched)
                {
                    matchedItems.Add(mainElement);
                }
                else
                {
                    unmatchedItems.Add(mainElement);
                }
            }

            // Generate the report
            GenerateReport(matchedItems, unmatchedItems);
        }
    }
}
    


