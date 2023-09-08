using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EMV_Parser
{
    public class DataElement
    {
        public string ElementName { get; set; }
        public string Value { get; set; }
        public ObservableCollection<DataSubElement> DataSubElements { get; set; }

        public DataElement()
        {
            DataSubElements = new ObservableCollection<DataSubElement>();
        }
    }
}
