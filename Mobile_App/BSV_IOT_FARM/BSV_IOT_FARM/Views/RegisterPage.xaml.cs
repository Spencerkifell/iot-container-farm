using BSV_IOT_FARM.Models;
using BSV_IOT_FARM.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new UserViewModel(new User()) { Navigation = this.Navigation };
        }
    }
}