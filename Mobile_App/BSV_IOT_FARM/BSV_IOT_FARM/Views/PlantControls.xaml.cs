using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantControls : ContentPage
    {
        public PlantControls()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }
    }
}