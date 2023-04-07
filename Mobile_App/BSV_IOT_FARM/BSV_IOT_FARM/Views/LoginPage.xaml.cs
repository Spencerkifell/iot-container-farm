using BSV_IOT_FARM.ViewModels;
using BSV_IOT_FARM.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new UserViewModel(new User()) { Navigation = this.Navigation };
        }
        
        protected override void OnAppearing()
        {
            App.MainViewModel.CurrentUser = null;
            base.OnAppearing();
        }
    }
}