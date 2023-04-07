using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSV_IOT_FARM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainerHubPage : TabbedPage
    { 
        public ContainerHubPage()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Override the OnDisappearing so that if the user navigates away from the page, the thread will be stopped.
        /// </summary>
        protected override void OnDisappearing()
        {
            if (App.MainViewModel.SelectedContainerFarm.ContainerFarm.CanRefresh != true)
                App.MainViewModel.SelectedContainerFarm.ContainerFarm.ShouldKillTask = true;
            base.OnDisappearing();
        }

        protected override async void OnAppearing()
        {
            if (Connectivity.NetworkAccess is not (NetworkAccess.Internet or NetworkAccess.Unknown or NetworkAccess.ConstrainedInternet))
                await DisplayAlert("Failed to Retrieve Device Twin", "Unable to retrieve device twin. Please check your network connectivity and try again.", "Ok");
            else
            {
                try
                {
                    // Meant to be used just so that when the user opens the container farm, they should have the updated values.
                    await App.MainViewModel.AzureRepository.RetrieveReportedProperties(App.MainViewModel.SelectedContainerFarm.ContainerFarm);
                }
                catch (Exception e)
                {
                    await DisplayAlert("Failed to Retrieve Device Twin", e.Message, "Ok");
                }
            }
            base.OnAppearing();
        }
    }
}