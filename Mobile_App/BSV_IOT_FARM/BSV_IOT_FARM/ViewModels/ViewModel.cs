/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development 3
 * 
 * View Model Base class used to store navigation and property changed event
 */

using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using BSV_IOT_FARM.Repos;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BSV_IOT_FARM.ViewModels
{
    /// <summary>
    /// This class is to be used as a base class for all view models in this application.
    /// It stores important data and methods for all view models.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        private const string FILENAME = "containerFarms.db3";
        private INavigation _navigation;
        protected static SqlRepo SqlRepo { get; } = new (Path.Combine(FileSystem.AppDataDirectory, FILENAME));

        /// <summary>
        /// Property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Fires property changed even of the given property.
        /// </summary>
        /// <param name="propertyName"> Name of property to fire the event for.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// Gets and sets the navigation of the view model.
        /// </summary>
        public INavigation Navigation
        {
            get => _navigation;
            set => _navigation = value;
        }
    }
}