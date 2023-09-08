using EMV_Parser;
using System.Collections.ObjectModel;
namespace EMV_Parser
{
    public class DGI
    {
        public string DGINumber { get; set; }
        public ObservableCollection<EMVTag> EMVTags { get; set; } = new ObservableCollection<EMVTag>();
    }
}