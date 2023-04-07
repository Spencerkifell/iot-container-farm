using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainerFormPage : ContentPage
    {
        public ContainerFormPage()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }
    }
}