using System.ComponentModel;

namespace Craft.UIElements.Reborn.GuiTest;

/// <summary>An editable draft of one arbitrary information string.</summary>
public class LabelInformationViewModel : INotifyPropertyChanged
{
    private string _text;

    public LabelInformationViewModel(string text = "") => _text = text;

    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
