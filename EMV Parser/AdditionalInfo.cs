using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EMV_Parser
{
    public class AdditionalInfo : INotifyPropertyChanged
    {
        private string _label;
        public string Label
        {
            get { return _label; }
            set { _label = value; OnPropertyChanged(); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
