using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartPage : ContentPage
    {
        public ChartPage()
        {
            BindingContext = App.MainViewModel;
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            App.MainViewModel.Chart = null;
            App.MainViewModel.ChartTitle = string.Empty;
            base.OnDisappearing();
        }
    }
}