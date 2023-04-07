using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainerCollectionPage : ContentPage
    {
        public ContainerCollectionPage()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
            App.MainViewModel.Navigation = Navigation;
        }

        protected override void OnAppearing()
        {
            App.MainViewModel.SelectedContainerFarm = null;
            base.OnAppearing();
        }
    }
}