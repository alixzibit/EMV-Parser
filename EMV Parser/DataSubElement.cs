using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EMV_Parser
{
    public class DataSubElement : INotifyPropertyChanged
    {
        public string SubElementName { get; set; }
        public string Tag { get; set; }
        public string Value { get; set; }

        private AdditionalInfo _additionalInfo;
        public AdditionalInfo AdditionalInfo
        {
            get { return _additionalInfo; }
            set { _additionalInfo = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
