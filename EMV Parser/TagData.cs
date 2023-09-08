using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TagData : INotifyPropertyChanged
{
    private string _elementName;
    public string ElementName
    {
        get => _elementName;
        set { _elementName = value; OnPropertyChanged(); }
    }

    private string _tag;
    public string Tag
    {
        get => _tag;
        set { _tag = value; OnPropertyChanged(); }
    }

    private string _category;
    public string Category
    {
        get => _category;
        set { _category = value; OnPropertyChanged(); }
    }

    private string _value;
    public string Value
    {
        get => _value;
        set { _value = value; OnPropertyChanged(); }
    }

    private string _parsedValue;
    public string ParsedValue
    {
        get => _parsedValue;
        set
        {
            _parsedValue = value;
            OnPropertyChanged();
        }
    }

    // Parameterless constructor
    public TagData() { }

    public TagData(string elementName, string tag, string category, string value)
    {
        ElementName = elementName;
        Tag = tag;
        Category = category;
        Value = value;
    }

    public override string ToString()
    {
        return $"{ElementName} - {Tag} - {Category} - {Value}";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
