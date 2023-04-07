/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development 3
 * 
 * Main View Model used to store a container farm and communicated with the views and stored data.
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BSV_IOT_FARM.Models;
using BSV_IOT_FARM.Repos;
using BSV_IOT_FARM.Views;
using Microcharts;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Location = BSV_IOT_FARM.Models.Location;

namespace BSV_IOT_FARM.ViewModels
{
    /// <summary>
    /// This Main View Model used to store a container farm and communicated with the views and stored data.
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private readonly AzureRepo _azureRepository = new();
        private UserViewModel _currentUser;
        private ContainerFarmViewModel _selectedContainerFarm;
        private ContainerFarmViewModel _formContainerFarm;
        private ContainerFarmViewModel _editedContainerFarm;
        private Chart _chart;
        private Thread _azureThread;
        private bool _isBusy;
        private int _totalPoints = 3;
        private string _chartTitle = string.Empty;
        
        /// <summary>
        /// Default constructor, initializes observable collection of container farms, loads container farms ad initializes commands.
        /// </summary>
        public MainViewModel()
        {
            ContainerFarms = new ObservableCollection<ContainerFarmViewModel>();
            
            // Command Initialization
            SelectionChangedCommand = new Command(SelectionChanged);
            AddContainerFarmCommand = new Command(AddContainerFarm);
            DeleteContainerFarmCommand = new Command<ContainerFarmViewModel>(DeleteContainerFarm);
            EditContainerFarmCommand = new Command<ContainerFarmViewModel>(EditContainerFarm);
            AddUserToContainerFarmCommand = new Command(AddUserToContainerFarm);
            SaveFormCommand = new Command(SaveForm);
            LogoutCommand = new Command(Logout);
            FetchTelemetryDataCommand = new Command(FetchTelemetryData);
            ReturnCommand = new Command(Return);
            DisplayProfileCommand = new Command(DisplayProfile);
            ModifyIntervalCommand = new Command(ModifyInterval);
            ModifyTelemetryCommand = new Command(ModifyTelemetry);
            RefreshTelemetryCommand = new Command(RefreshTelemetry);
            GetPropertyGraphCommand = new Command(GetPropertyLineChart);
            SetAmountOfDataPointsCommand = new Command(SetAmountOfDataPoints);
            GenerateMapCommand = new Command(GenerateMap);

            // Initializes the event handler for connectivity changes.
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        /// <summary>
        /// Event handler for connectivity changes. Will prompt the user dependant on their connectivity status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            switch (Connectivity.NetworkAccess)
            {
                case NetworkAccess.None:
                    await Navigation.NavigationStack.Last().DisplayAlert("Connectivity Alert", "You won't be able to retrieve telemetry since your device is not connected to a network.", "OK");
                    break;
                case NetworkAccess.Unknown:
                    await Navigation.NavigationStack.Last().DisplayAlert("Connectivity Alert", "You might be not able to retrieve telemetry since your device is connected to an Unknown network.", "OK");
                    break;
                case NetworkAccess.ConstrainedInternet:
                    await Navigation.NavigationStack.Last().DisplayAlert("Connectivity Alert", "You might not be able to retrieve telemetry since your device is connected to an Constrained network.", "OK");
                    break;
                case NetworkAccess.Local:
                    await Navigation.NavigationStack.Last().DisplayAlert("Connectivity Alert", "You won't be able to retrieve telemetry since your device is connected to an Local network.", "OK");
                    break;
            }
        }
        
        #region Commands

        // Public command properties for binding to the UI
        public ICommand SelectionChangedCommand { get; }
        public ICommand AddContainerFarmCommand { get; }
        public ICommand DeleteContainerFarmCommand { get; }
        public ICommand EditContainerFarmCommand { get; }
        public ICommand AddUserToContainerFarmCommand { get; }
        public ICommand SaveFormCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand RefreshTelemetryCommand { get; }
        public ICommand FetchTelemetryDataCommand { get; }
        public ICommand ReturnCommand { get; }
        public ICommand DisplayProfileCommand { get; }
        public ICommand ModifyIntervalCommand { get; }
        public ICommand ModifyTelemetryCommand { get; }
        public ICommand GetPropertyGraphCommand { get; }
        public ICommand SetAmountOfDataPointsCommand { get; }
        public ICommand GenerateMapCommand { get; }

        #endregion

        #region Public/Private Methods

        /// <summary>
        /// Method used to create a new thread to fetch telemetry data.
        /// </summary>
        public void CreateNewThread()
        {
            // If the thread exists, then it should be killed so that it can be replaced.
            AzureThread?.Abort();
            // Re-initialize the thread since threads can't be re-used. Thread will be set to a background task so that it runs as a Daemon thread.
            AzureThread = new Thread(RetrieveTelemetryData) { IsBackground = true };
        }
        
        /// <summary>
        /// Async method to retrieve telemetry data from the azure repository. This method is meant to be run in a background thread.
        /// </summary>
        private async void RetrieveTelemetryData()
        {
            try
            {
                var continueRefresh = true;
                
                // Referenced from: https://stackoverflow.com/questions/59007429/raise-a-method-every-20-minutes-in-xamarin
                // Essentially every time the interval passes it calls an anonymous function to on property change the selected container farm.
                Device.StartTimer(TimeSpan.FromSeconds(SelectedContainerFarm.ContainerFarm.TelemetryInterval), () =>
                {
                    Task.Run(() =>
                    {
                        OnPropertyChanged(nameof(SelectedContainerFarm));
                    });
                    // If this returns true, the timer will start again, otherwise it will end.
                    return continueRefresh; 
                });
                
                var retrieveTelemetryTask = _azureRepository.RetrieveTelemetryData(SelectedContainerFarm.ContainerFarm);
                    
                await retrieveTelemetryTask.ContinueWith((task) =>
                {
                    // Whenever the task ends, the timer is told that it should not refresh anymore.
                    if (task.Exception != null && task.Exception.InnerException != null) 
                        // Since the thread is going to be killed and can't display the error to the user, we need to invoke the method on the main thread.
                        MainThread.InvokeOnMainThreadAsync(() => Navigation.NavigationStack.Last().DisplayAlert("Refresh Cancelled", $"{task.Exception.InnerException.Message}", "Ok"));
                    continueRefresh = false;
                });
            }
            catch (Exception e)
            {
                // ignored since we don't want the app to crash if the thread is aborted.
            }
        }
        
        /// <summary>
        /// Private method to be utilized in a command whenever a different container farm is selected.
        /// </summary>
        private async void SelectionChanged()
        {
            // We check if the last page pushed onto the navigation stack is the collection page because there is a Navigation issue present.
            if (SelectedContainerFarm != null && Navigation.NavigationStack.Last().GetType() == typeof(ContainerCollectionPage))
                await Navigation.PushAsync(new ContainerHubPage());
        }

        /// <summary>
        /// Will set the current form container farm as the new container farm and then open it in an editor.
        /// </summary>
        private void AddContainerFarm()
        {
            FormContainerFarm = new ContainerFarmViewModel(new ContainerFarm()) { Navigation = Navigation };
            Navigation.PushAsync(new ContainerFormPage());
        }

        /// <summary>
        /// Private method to be utilized in a command whenever a container farm is deleted.
        /// </summary>
        /// <param name="containerFarm"> Takes in the container farm to delete. </param>
        private void DeleteContainerFarm(ContainerFarmViewModel containerFarm)
        {
            // Surrounded in a try catch block to prevent the app from crashing if the user tries to delete a container farm that doesn't exist.
            try
            {
                SqlRepo.DeleteContainerFarm(containerFarm.ContainerFarm);
                ContainerFarms.Remove(containerFarm);
            
                // Since the container is owned by the user, we also need to refresh the user's container farms. (Since this is already done within the model, we just need to reflect the change in the view model.)
                CurrentUser.ContainerFarms.Remove(containerFarm.ContainerFarm);
            }
            catch (Exception e)
            {
                Navigation.NavigationStack.Last().DisplayAlert("Error Deleting Container Farm", e.Message, "OK");
            }
        }

        /// <summary>
        /// Private method to be utilized in a command whenever an admin wants to edit a container farm.
        /// </summary>
        /// <param name="containerFarm"></param>
        private void EditContainerFarm(ContainerFarmViewModel containerFarm)
        {
            // Surrounded in a try catch block in case an error occurs when accessing data from the database.
            try
            {
                containerFarm.ContainerFarm = SqlRepo.GetContainerFarmById(containerFarm.ContainerFarm.Id);
                EditedContainerFarm = containerFarm;
                Navigation.PushAsync(new ContainerSettingsPage());
            }
            catch (Exception e)
            {
                Navigation.NavigationStack.Last().DisplayAlert("Error Editing Container Farm", e.Message, "OK");
            }
        }

        /// <summary>
        /// Private method to be utilized in a command when the manager wants to add a user to a specific container farm.
        /// </summary>
        private async void AddUserToContainerFarm()
        {
            // Surrounded in a try catch block in case an error occurs when accessing data from the database.
            try
            {
                var result = await Navigation.NavigationStack.Last().DisplayPromptAsync("Add User to Container Farm", "What's the username of the user you would like to add?");

                // If the user cancels the prompt or does not enter a username/enters whitespace, we do not want to continue.   
                if (result == null || result.Trim().Length == 0)
                {
                    await Navigation.NavigationStack.Last().DisplayAlert("Error", "Please enter a valid username.", "OK");
                    return;
                }
            
                // Tries to get the user from the database based off the inputted username.
                var user = SqlRepo.GetUserByUserName(result);
            
                if (user == null)
                {
                    await Navigation.NavigationStack.Last().DisplayAlert("Error", "User not found.", "OK");
                    return;
                }
            
                // If the user is already a member of the container farm, we do not want to continue.
                if (user.ContainerFarms.Contains(EditedContainerFarm.ContainerFarm))
                {
                    await Navigation.NavigationStack.Last().DisplayAlert("Error", "User is already a member of this container farm.", "OK");
                    return;
                }
                
                user.ContainerFarms.Add(EditedContainerFarm.ContainerFarm);
                EditedContainerFarm.ContainerFarm.Users.Add(user);
                SqlRepo.AddOrUpdateUser(user);
            }
            catch (Exception e)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Error Adding User to Container Farm", e.Message, "OK");
            }
        }

        /// <summary>
        /// Private method to be utilized in a command whenever a container farm is added and saved.
        /// </summary>
        private async void SaveForm()
        {
            // If the required fields are not filled out.
            if (FormContainerFarm.IsInvalid)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed to Create Container Farm", "Please verify that all required fields have been filled.", "Ok");
                return;
            }

            // If the connection to the Azure database is not valid. (Formatting wise, not connectivity wise)
            if (FormContainerFarm.IsConnectionInvalid)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed to Create Container Farm", "Please verify that your EventHub name and connection string are valid.", "Ok");
                return;
            }

            // If the container farm is new, then we need to set its status to false.
            if (FormContainerFarm.IsNew)
                FormContainerFarm.IsNew = false;

            // Surrounded in a try catch block in case an error occurs when accessing data from the database.
            try
            {
                SqlRepo.AddOrUpdateContainerFarm(FormContainerFarm.ContainerFarm);
                
                // If the container farm is new, then we need to add the container farm to the collection of container farms.
                if (!ContainerFarms.Contains(FormContainerFarm))
                {
                    ContainerFarms.Add(FormContainerFarm);
                    CurrentUser.ContainerFarms.Add(FormContainerFarm.ContainerFarm);
                    SqlRepo.AddOrUpdateUser(CurrentUser.User);
                }
            }
            catch (Exception e)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed to Create Container Farm", e.Message, "Ok");
            }

            await Navigation.PopAsync();
        }

        /// <summary>
        /// Private method to be utilized in a command when the user wishes to log out.
        /// </summary>
        private void Logout()
        {
            Navigation.PopToRootAsync();
        }

        /// <summary>
        /// Private method to be utilized in a command when the user requests to refresh the telemetry data.
        /// </summary>
        private async void FetchTelemetryData()
        {
            // If the network is not available, then we cannot fetch the telemetry data.
            if (Connectivity.NetworkAccess is not (NetworkAccess.Internet or NetworkAccess.Unknown or NetworkAccess.ConstrainedInternet))
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Refresh Failure", "Unable to refresh, please verify your network connectivity. Please try again later.", "Ok");
                return;
            }

            // If a refresh is already in progress, then we cannot fetch the telemetry data.
            if (!SelectedContainerFarm.ContainerFarm.CanRefresh)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Refresh Failure", "Unable to refresh, since a refresh is currently in progress. Please try again later.", "Ok");
                return;
            }

            if (Connectivity.NetworkAccess is (NetworkAccess.ConstrainedInternet or NetworkAccess.Unknown))
                await Navigation.NavigationStack.Last().DisplayAlert("Refresh Warning", "An attempt to refresh the data will be made. Since network connectivity is constrained or unknown, this may or may not work.", "Ok");

            await Navigation.NavigationStack.Last().DisplayAlert("Refresh Started", "Data is now being refreshed.", "Ok");

            SelectedContainerFarm.ShouldKillTask = false;

            try
            {
                // Meant to be used just so that when the user opens the container farm, they should have the updated values.
                await AzureRepository.RetrieveReportedProperties(SelectedContainerFarm.ContainerFarm);
            }
            catch (Exception e)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed to Retrieve Device Twin State", e.Message, "Ok");
            }
            
            CreateNewThread();
            AzureThread.Start();
        }

        /// <summary>
        /// Private method to be utilized in a command whenever the user wants to return to the previous page.
        /// </summary>
        private async void Return()
        {
            await Navigation.PopAsync();
        }

        private async void DisplayProfile()
        {
            await Navigation.NavigationStack.Last().DisplayAlert("Profile", $"Name: {CurrentUser.FirstName}\nUsername: {CurrentUser.Username}\nTotal Farms: {CurrentUser.TotalFarms}\nRole: {CurrentUser.User.RoleString}", "Return");
        }

        private void RefreshTelemetry()
        {
            if (!IsBusy)
                return;
            
            OnPropertyChanged(nameof(SelectedContainerFarm));
            IsBusy = false;
        }

        /// <summary>
        /// Will retrieve the dataset of container farms from the database.
        /// </summary>
        public void LoadContainerFarms()
        {
            ContainerFarms.Clear();
            var containerFarms = CurrentUser.ContainerFarms;
            containerFarms.ForEach(containerFarm => ContainerFarms.Add(new ContainerFarmViewModel(containerFarm) { IsNew = false }));
        }

        /// <summary>
        /// Private method to be utilized in a command when the user wishes modify telemetry interval.
        /// </summary>
        private async void ModifyInterval()
        {
           await Navigation.PushAsync(new IntervalPage());
        }

        /// <summary>
        /// Private method to be utilized in a command when the user saves a new interval.
        /// </summary>
        private async void ModifyTelemetry()
        {
            var interval = SelectedContainerFarm.TelemetryInterval;

            // Interval validation so that the user cannot enter an invalid input
            if (interval <= 0)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Error Setting Interval", "Unable to change the interval. Please input a positive numerical value.", "Ok");
                return;
            }

            await _azureRepository.PatchTelemetryInterval(SelectedContainerFarm.ContainerFarm, interval);
            await Navigation.PopAsync();
        }

        /// <summary>
        /// private method used to modify device twin.
        /// </summary>
        /// <param name="turnOn">True if the light is to be turned on, false otherwise.</param>
        /// <returns>Task of asyncronus operation.</returns>
        private async Task SwitchLight(bool turnOn)
        {
            if(turnOn)
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.LightOn);
            else
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.LightOff);
        }

        /// <summary>
        /// private method used to modify device twin.
        /// </summary>
        /// <param name="turnOn">True if the fan is to be turned on, false otherwise.</param>
        /// <returns>Task of asyncronus operation.</returns>
        private async Task SwitchFan(bool turnOn)
        {
            if (turnOn)
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.FanOn);
            else
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.FanOff);
        }

        /// <summary>
        /// private method used to modify device twin.
        /// </summary>
        /// <param name="open">True if the door is to be unlocked, false otherwise.</param>
        /// <returns>Task of asyncronus operation.</returns>
        private async Task SwitchDoor(bool @lock)
        {
            if (@lock)
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.DoorClose);
            else
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.DoorOpen);
        }

        /// <summary>
        /// private method used to modify device twin.
        /// </summary>
        /// <param name="turnOn">True if the buzzer is to be turned on, false otherwise.</param>
        /// <returns>Task of asyncronus operation.</returns>
        private async Task SwitchBuzzer(bool turnOn)
        {
            if (turnOn)
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.BuzzerOn);
            else
                await _azureRepository.PatchActuatorDeviceTwin(SelectedContainerFarm.ContainerFarm, AzureRepo.DeviceTwinState.BuzzerOff);

        }

        /// <summary>
        /// Once triggered on selected index changed, it will grab the requested property and generate a line chart based of the telemetry history.
        /// </summary>
        /// <param name="sender"> Takes in the picker in order to use the string value. </param>
        private void GetPropertyLineChart(object sender)
        {
            var picker = (Picker) sender;
            var selectedIndex = picker.SelectedIndex;
            
            if (selectedIndex is -1) 
                return;
            
            var property = picker.ItemsSource[selectedIndex].ToString();

            try
            {
                Chart = ChartsRepo.GetLineChart(SelectedContainerFarm.ContainerFarm.TelemetryHistory.ToList(), property, _totalPoints);
                ChartTitle = $"{property} Chart";
            }
            catch (Exception e)
            {
                Navigation.NavigationStack.Last().DisplayAlert("Issue Generating Chart", e.Message, "Ok");
            }
            finally
            {
                // Sets it back to -1 so the user can see the prompt again.
                picker.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Method to be utilized in a command so that the user can input the amount of data points they would like to visualize.
        /// </summary>
        private async void SetAmountOfDataPoints()
        {
            // Limited to 50 because beyond 50, the application begins to slow down due to computation.
            var result = await Navigation.NavigationStack.Last().DisplayPromptAsync("Graph Data Points", "How many data points would you like to view? (0-50)");

            if (result is null || result.Trim().Length == 0)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Error: Graph Data Points", "Please input a valid value.", "Ok");
                return;
            }
            
            if (!int.TryParse(result, out int amountOfDataPoints))
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Error: Graph Data Points", "Please input a valid numerical value.", "Ok");
                return;
            }

            switch (amountOfDataPoints)
            {
                case < 1:
                    await Navigation.NavigationStack.Last().DisplayAlert("Error: Graph Data Points", "Please input a positive value.", "Ok");
                    return;
                case > 50:
                    await Navigation.NavigationStack.Last().DisplayAlert("Error: Graph Data Points", "Please input a value less than or equal to 50.", "Ok");
                    return;
                default:
                    _totalPoints = amountOfDataPoints;
                    break;
            }
        }

        /// <summary>
        /// Method to be utilized in a command to generate a map for the user based off the container farm's longitude and latitude.
        /// </summary>
        private async void GenerateMap()
        {
            if (SelectedContainerFarm.Telemetry.Latitude is null || SelectedContainerFarm.Telemetry.Longitude is null || (SelectedContainerFarm.Telemetry.Longitude == 0 && SelectedContainerFarm.Telemetry.Latitude == 0))
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed to Generate Map", "Unable to generate map since longitude or latitude values may be unavailable.", "Ok");
                return;
            }

            // Once validation is complete, the values are safely parsed into doubles to be transformed into a Location object.
            var longitude = (double) SelectedContainerFarm.Telemetry.Longitude;
            var latitude = (double) SelectedContainerFarm.Telemetry.Latitude;

            var currentLocation = new Location(longitude, latitude);
            
            // The only way to bind a map in MVVM is using the items source property, by clearing the location we are able to view our current position.
            Locations.Clear();
            Locations.Add(currentLocation);

            await Navigation.PushAsync(new MapPage());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the observable collection of container famrs.
        /// </summary>
        public ObservableCollection<ContainerFarmViewModel> ContainerFarms { get; private set; }
        
        public AzureRepo AzureRepository => _azureRepository;

        /// <summary>
        /// Location property to be used when utilizing the map with Longitude/Latitude
        /// </summary>
        public List<Location> Locations { get; private set; } = new List<Location>();
        
        /// <summary>
        /// Gets the Chart for the measurement properties
        /// </summary>
        public Chart Chart
        {
            get => _chart;
            set
            {
                if (value == _chart)
                    return;

                _chart = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets and sets the form for the container farm.
        /// </summary>
        public ContainerFarmViewModel FormContainerFarm
        {
            get => _formContainerFarm;
            set
            {
                if (value == _formContainerFarm)
                    return;

                _formContainerFarm = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets and sets the selected container farm.
        /// </summary>
        public ContainerFarmViewModel SelectedContainerFarm
        {
            get => _selectedContainerFarm;
            set
            {
                if (value == _selectedContainerFarm)
                    return;

                _selectedContainerFarm = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets and sets the edited container farm.
        /// </summary>
        public ContainerFarmViewModel EditedContainerFarm
        {
            get => _editedContainerFarm;
            set
            {
                if (value == _editedContainerFarm)
                    return;
                
                _editedContainerFarm = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value == _isBusy)
                    return;

                _isBusy = value;
                OnPropertyChanged();
            }
        }
        
        public string ChartTitle
        {
            get => _chartTitle;
            set
            {
                if (value == _chartTitle)
                    return;
                
                _chartTitle = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets and sets the current user's data.
        /// </summary>
        public UserViewModel CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }
        
        /// <summary>
        /// Public property to get the AzureThread that is meant to run in the background.
        /// </summary>
        public Thread AzureThread
        {
            get => _azureThread;
            set => _azureThread = value;
        }
        
        /// <summary>
        /// Gets and sets the boolean value for fan state in the telemetry of the selected container farm.
        /// </summary>
        public bool FanIsOn
        {
            get => SelectedContainerFarm.Telemetry.FanIsActive == null ? false : SelectedContainerFarm.Telemetry.FanIsActive == true;
            set
            {
                SwitchFan(value);
                SelectedContainerFarm.Telemetry.FanIsActive = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets and sets the boolean value for light state in the telemetry of the selected container farm.
        /// </summary>
        public bool LightIsOn
        {
            get => SelectedContainerFarm.Telemetry.LightIsActive == null ? false : SelectedContainerFarm.Telemetry.LightIsActive == true ;
            set
            {
                SwitchLight(value);
                SelectedContainerFarm.Telemetry.LightIsActive = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Gets and sets the boolean value for buzzer state in the telemetry of the selected container farm.
        /// </summary>
        public bool BuzzerIsOn
        {
            get => SelectedContainerFarm.Telemetry.BuzzerIsActive == null ? false : SelectedContainerFarm.Telemetry.BuzzerIsActive == true;
            set
            {
                SwitchBuzzer(value);
                SelectedContainerFarm.Telemetry.BuzzerIsActive = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Gets and sets the boolean value for door lock state in the telemetry of the selected container farm.
        /// </summary>
        public bool DoorIsLocked
        {
            get => SelectedContainerFarm.Telemetry.DoorIsLocked == null ? false : SelectedContainerFarm.Telemetry.DoorIsLocked == true;
            set
            {
                SwitchDoor(value);
                SelectedContainerFarm.Telemetry.DoorIsLocked = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}