using StructureMap;
using Craft.GeometryEditor3D.ViewModel;

namespace Craft.GeometryEditor3D.UI.WPF
{
    public class BootStrapper
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return Container.For<MainWindowViewModelRegistry>().GetInstance<MainWindowViewModel>();
            }
        }
    }
}
