//using System.Collections.Generic;
//using System.Collections.ObjectModel;

//namespace EMV_Parser
//{
//    public class DataGroup
//    {
//        public string GroupName { get; set; }
//        public ObservableCollection<DataElement> DataElements { get; set; }

//        public DataGroup()
//        {
//            DataElements = new ObservableCollection<DataElement>();
//        }

//    }
//}

using EMV_Parser;
using System.Collections.ObjectModel;
namespace EMV_Parser
{
    public class DataGroup
    {
        public string GroupName { get; set; }
        public ObservableCollection<DataSubElement> DataSubElements { get; set; } // Change this line

        public DataGroup()
        {
            DataSubElements = new ObservableCollection<DataSubElement>(); // Change this line
        }
    }
}

