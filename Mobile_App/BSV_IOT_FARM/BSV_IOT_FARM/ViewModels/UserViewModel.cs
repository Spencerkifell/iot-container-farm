/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development 3
 * 
 * This User ViewModel used to store a user in order to be effectively rendered within the UI.
 */

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BSV_IOT_FARM.Models;
using BSV_IOT_FARM.Views;
using Xamarin.Forms;

namespace BSV_IOT_FARM.ViewModels
{
    /// <summary>
    /// This User ViewModel used to store a user in order to be effectively rendered within the UI.
    /// </summary>
    public class UserViewModel : ViewModel
    {
        private User _user;
        
        // Validators
        private bool _isFleetManagerChecked;
        private bool _isFirstNameInvalid;
        private bool _isUsernameInvalid;
        private bool _isPasswordInvalid;
        
        public UserViewModel(User user)
        {
            _user = user;
            
            // Command Initialization
            RegisterCommand = new Command(Register);
            LoginCommand = new Command(Login);
            OpenRegisterCommand = new Command(OpenRegister);
        }

        #region Commands

        /// <summary>
        /// Command property for when the user is ready to register a new user.
        /// </summary>
        public ICommand RegisterCommand { get; }
        
        /// <summary>
        /// Command property for when the user is ready to login to the application.
        /// </summary>
        public ICommand LoginCommand { get; }
        
        /// <summary>
        /// Command property for when the user wants to register a new user.
        /// </summary>
        public ICommand OpenRegisterCommand { get; }

        #endregion
        
        #region Properties
        
        /// <summary>
        /// Calculated property representing if the user is a fleet manager based on the current user's role.
        /// </summary>
        public bool IsFleetManager => Role == (int) UserRole.FleetManager;
        
        /// <summary>
        /// Calculated property that determines the validity of the user's registration input fields.
        /// </summary>
        public bool IsInvalid => IsFirstNameInvalid || IsUsernameInvalid || IsPasswordInvalid;

        /// <summary>
        /// Calculated property that returns the user's total amount of containers.
        /// </summary>
        public int TotalFarms => _user.ContainerFarms.Count;
        
        /// <summary>
        /// Public property representing the current user object being used within the viewmodel.
        /// </summary>
        public User User
        {
            get => _user;
            set
            {
                if (_user == value)
                    return;
                
                _user = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing the current user's ID being used within the viewmodel.
        /// </summary>
        public int Id
        {
            get => _user.Id;
            set
            {
                if (_user.Id == value) 
                    return;
                
                _user.Id = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing the current user's first name being used within the viewmodel.
        /// </summary>
        public string FirstName
        {
            get => _user.FirstName;
            set
            {
                if (_user.FirstName == value) 
                    return;
                
                _user.FirstName = value;
                OnPropertyChanged();              
            }
        }

        /// <summary>
        /// Public property representing the validity of the current user's first name.
        /// </summary>
        public bool IsFirstNameInvalid
        {
            get => _isFirstNameInvalid;
            set
            {
                if (_isFirstNameInvalid == value)
                    return;
                
                _isFirstNameInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the current user's username being used within the viewmodel.
        /// </summary>
        public string Username
        {
            get => _user.Username;
            set
            {
                if (_user.Username == value) 
                    return;
                
                _user.Username = value;
                OnPropertyChanged();              
            }
        }
        
        /// <summary>
        /// Public property representing the validity of the current user's username.
        /// </summary>
        public bool IsUsernameInvalid
        {
            get => _isUsernameInvalid;
            set
            {
                if (_isUsernameInvalid == value)
                    return;
                
                _isUsernameInvalid = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Public property representing the current user's password being used within the viewmodel.
        /// </summary>
        public string Password
        {
            get => _user.Password;
            set
            {
                if (_user.Password == value) 
                    return;
                
                _user.Password = value;
                OnPropertyChanged();              
            }
        }
        
        /// <summary>
        /// Public property representing the validity of the current user's password.
        /// </summary>
        public bool IsPasswordInvalid
        {
            get => _isPasswordInvalid;
            set
            {
                if (_isPasswordInvalid == value)
                    return;
                
                _isPasswordInvalid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property to be utilized when registering a new user to determine if the user is a fleet manager. (Utilized in checkbox)
        /// </summary>
        public bool IsFleetManagerChecked
        {
            get => _isFleetManagerChecked;
            set
            {
                if (_isFleetManagerChecked == value)
                    return;
                
                _isFleetManagerChecked = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Public property representing the integer value of the current user's role.
        /// </summary>
        public int Role
        {
            get => _user.Role;
            set
            {
                if (_user.Role == value) 
                    return;
                
                _user.Role = value;
                OnPropertyChanged();              
            }
        }

        /// <summary>
        /// Public property representing the user's current container farms.
        /// </summary>
        public ObservableCollection<ContainerFarm> ContainerFarms
        {
            get => _user.ContainerFarms;
            set
            {
                if (_user.ContainerFarms == value) 
                    return;
                
                _user.ContainerFarms = value;
                OnPropertyChanged();              
            }
        }

        #endregion

        #region Public/Private Methods

        /// <summary>
        /// To be utilized in a command when the user is ready to register based on the validity of their inputted fields.
        /// </summary>
        private async void Register()
        {
            // Checks if the inputted fields are valid based off the bound properties.
            if (IsInvalid)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed To Register", "Please validate and fill in all fields", "Ok");
                return;
            }

            // Wrapped in a try catch block to handle any errors that may occur when accessing the database.
            try
            {
                User.Role = IsFleetManagerChecked ? (int) UserRole.FleetManager : (int) UserRole.FarmTechnician;
                SqlRepo.AddOrUpdateUser(User);
            }
            catch (Exception e)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed To Register", e.Message, "Ok");
                return;
            }
            
            await Navigation.PopAsync();
        }

        /// <summary>
        /// To be utilized in a command when the user click the button to login to their account using their credentials.
        /// </summary>
        private async void Login()
        {
            // Checks if the user's inputted credentials are valid.
            if (IsUsernameInvalid || IsPasswordInvalid)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed To Login", "Please validate and fill in all fields", "Ok");
                return;
            }
            
            // Wrapped in a try catch block to handle any errors that may occur when accessing the database.
            try
            {
                // Queries the database using the database repository in order to fetch the user object that matches the username and password.
                var user = SqlRepo.GetUserByLoginIn(Username, Password);
                
                if (user == null)
                {
                    await Navigation.NavigationStack.Last().DisplayAlert("Failed To Login", "Username or Password is incorrect", "Ok");
                    return;
                }
                
                UpdateFields(user);
                
                // Sets the MainViewModels current user to this instance of the user viewModel since the data is populated accordingly.
                App.MainViewModel.CurrentUser = this;
                App.MainViewModel.LoadContainerFarms();
                await Navigation.PushAsync(new ContainerCollectionPage());
            }
            catch (Exception e)
            {
                await Navigation.NavigationStack.Last().DisplayAlert("Failed To Login", e.Message, "Ok");
            }
        }

        /// <summary>
        /// To be utilized in a command when the user wants to register a new user.
        /// </summary>
        private void OpenRegister()
        {
            Navigation.PushAsync(new RegisterPage());
        }

        /// <summary>
        /// Updates the fields of the view model's user property based off the user passed in.
        /// </summary>
        /// <param name="user"> Takes in an existing user to update its values. </param>
        private void UpdateFields(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            Username = user.Username;
            Password = user.Password;
            Role = user.Role;
            ContainerFarms = user.ContainerFarms;
        }

        #endregion
    }
}