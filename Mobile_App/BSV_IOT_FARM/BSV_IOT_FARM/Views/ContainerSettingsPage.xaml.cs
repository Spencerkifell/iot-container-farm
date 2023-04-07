using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainerSettingsPage : ContentPage
    {
        public ContainerSettingsPage()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }
    }
}