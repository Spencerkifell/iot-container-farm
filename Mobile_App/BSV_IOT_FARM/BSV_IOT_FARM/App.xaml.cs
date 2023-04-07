using BSV_IOT_FARM.ViewModels;
using BSV_IOT_FARM.Views;
using Xamarin.Forms;

namespace BSV_IOT_FARM
{
    public partial class App : Application
    {
        public static MainViewModel MainViewModel { get; set; } = new MainViewModel();

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
