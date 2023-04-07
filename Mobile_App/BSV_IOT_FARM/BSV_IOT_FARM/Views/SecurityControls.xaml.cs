using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecurityControls : ContentPage
    {
        public SecurityControls()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }
    }
}