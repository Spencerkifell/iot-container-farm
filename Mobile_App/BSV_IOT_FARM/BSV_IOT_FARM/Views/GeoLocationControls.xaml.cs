using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoLocationControls : ContentPage
    {
        public GeoLocationControls()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }
    }
}