using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntervalPage : ContentPage
    {
        public IntervalPage()
        {
            InitializeComponent();
            BindingContext = App.MainViewModel;
        }
    }
}