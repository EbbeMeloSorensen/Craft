using System.Collections.Generic;
using Craft.ViewModels.Charts;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab2
{
    public class Tab2ViewModel : ViewModelBase
    {
        public PieChartViewModel PieChartViewModel { get; private set; }

        public Tab2ViewModel()
        {
            PieChartViewModel = new PieChartViewModel();

            var title = "Bamses Billedbog";
            var diameter = 400;
            var distribution = new Dictionary<string, double>
            {
                { "Bamse", 0.0 },
                { "Kylling", 0.0 },
                { "Luna", 5.0 },
                //{ "Aske", 30.0 },
                //{ "Kasper", 5.0 },
                //{ "Jesper", 5.0 },
                //{ "Jonathan", 5.0 },
                //{ "Løven", 5.0 },
                //{ "Lars", 5.0 },
                //{ "Gitte", 5.0 },
                //{ "Kim", 5.0 },
                //{ "Dorthe", 5.0 },
                //{ "Ebbe", 5.0 },
                //{ "Uffe", 5.0 },
                //{ "Michael", 5.0 },
                //{ "Bjarke", 5.0 },
                //{ "Hugo", 5.0 },
                //{ "Egon", 5.0 }
            };

            PieChartViewModel.Initialize(title, diameter, distribution);
        }
    }
}
