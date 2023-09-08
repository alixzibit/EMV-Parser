//using EMV_Parser;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//namespace EMV_Parser
//{
//    public class CardType
//    {
//        public string CardTypeName { get; set; }
//        public ObservableCollection<DataGroup> DataGroups { get; set; }

//        public CardType()
//        {
//            DataGroups = new ObservableCollection<DataGroup>();
//        }
//    }


//}

using EMV_Parser;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EMV_Parser
{
    public class CardType
    {
        public string TypeName { get; set; }
        public ObservableCollection<DataGroup> DataGroups { get; set; }

        public CardType()
        {
            DataGroups = new ObservableCollection<DataGroup>();
        }
    }
}
