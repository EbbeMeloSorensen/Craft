using GalaSoft.MvvmLight;

namespace Craft.Simulation.Reborn.GuiTest;

public class DummyViewModel : ViewModelBase
{
    private string _greeting;

    public string Greeting
    {  get => _greeting;
       set
       {
            _greeting = value;
            RaisePropertyChanged();
       }
    }

    public DummyViewModel()
    {
        _greeting = "Hello from DummyViewModel";
    }
}
