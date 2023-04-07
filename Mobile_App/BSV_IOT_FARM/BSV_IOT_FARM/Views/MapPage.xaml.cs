using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            BindingContext = App.MainViewModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            // Only thing that couldn't be done in MVVM 
            CurrentMap.MoveToRegion(new MapSpan(App.MainViewModel.Locations.FirstOrDefault()!.Position, 0.01, 0.01));
            base.OnAppearing();
        }
    }
}