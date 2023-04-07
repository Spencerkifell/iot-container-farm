/*
 * BSV - Team #12
 * Winter 2022 - May 20, 2022
 * Application Development 3
 * 
 * Container Farm Repository is used to store multiple Container Farms.
 */

using System;
using System.Linq;
using BSV_IOT_FARM.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace BSV_IOT_FARM.Repos
{
    /// <summary>
    /// This class is used to store multiple container farms and their data.
    /// </summary>
    public class SqlRepo
    {
        // Private Fields
        private readonly SQLiteConnection _connection;
        
        public SqlRepo(string dbPath)
        {
            _connection = new SQLiteConnection(dbPath);
            // Create the Container Farm table if it doesn't exist.
            _connection.CreateTable<ContainerFarm>();
            // Create the User table if it doesn't exist.
            _connection.CreateTable<User>();
            // Create the many to many relationship between the Container Farm and the User.
            _connection.CreateTable<ContainerFarmUser>();
        }
        
        /// <summary>
        /// Takes in any container farm and dependant on the id parameter the quote will either be updated or inserted into the database table.
        /// </summary>
        /// <param name="containerFarm"> Takes in any container farm field. </param>
        /// <exception cref="ArgumentException"> Throws an error if there is an issue inserting or updating the selected container farm. </exception>
        public void AddOrUpdateContainerFarm(ContainerFarm containerFarm)
        {
            try
            {
                // If the container's id is the default integer of 0, then we know that the quote is new.
                if (containerFarm.Id == 0)
                    _connection.InsertWithChildren(containerFarm);
                else
                    _connection.UpdateWithChildren(containerFarm);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Takes in an existing user and dependant on the id parameter the user will either be updated or inserted into the database table.
        /// </summary>
        /// <param name="user"> Takes in an existing user object </param>
        /// <exception cref="ArgumentException"> If there are any issues communicating with the database an exception is thrown. </exception>
        public void AddOrUpdateUser(User user)
        {
            try
            {
                if (user.Id == 0)
                {   
                    // If the user is new and the username already exists in the database, then throw an exception to be bubbled up.
                    if (UsernameExists(user.Username))
                        throw new ArgumentException("A user with that username already exists.");
                    _connection.InsertWithChildren(user);
                }
                else
                    _connection.UpdateWithChildren(user);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Takes in a string representing the username and verifies if the username is already in use.
        /// </summary>
        /// <param name="userName"> String representing the username. </param>
        /// <returns> Returns true or false dependant if the username exists in the database. </returns>
        /// <exception cref="ArgumentException"> If there are any issues communicating with the database an error is thrown. </exception>
        private bool UsernameExists(string userName)
        {
            try
            {
                return _connection.GetAllWithChildren<User>().FirstOrDefault(currentUser => currentUser.Username.Equals(userName)) != null;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
        
        /// <summary>
        /// Takes in a string representing the username of a user and returns the user object if it exists.
        /// </summary>
        /// <param name="userName"> String representing the username. </param>
        /// <returns> Returns a user object or null dependant on if the user exists. </returns>
        /// <exception cref="ArgumentException"> If there are any issues communicating with the database an error is thrown. </exception>
        public User GetUserByUserName(string userName)
        {
            try
            {
                return _connection.GetAllWithChildren<User>().FirstOrDefault(currentUser => currentUser.Username.Equals(userName));
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Takes in a string representing the username and password and verifies if the user exists with the given credentials.
        /// </summary>
        /// <param name="username"> String representing the username. </param>
        /// <param name="password"> String representing the password. </param>
        /// <returns> Returns a user object or null dependant on if the user exists with the given credentials. </returns>
        /// <exception cref="ArgumentException"> If there are any issues communicating with the database an error is thrown. </exception>
        public User GetUserByLoginIn(string username, string password)
        {
            try
            {
                return _connection.GetAllWithChildren<User>().FirstOrDefault(user => user.Username.Equals(username) && user.Password.Equals(password));
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
        
        /// <summary>
        /// Takes in an id and returns the associated container farm object.
        /// </summary>
        /// <param name="id"> Integer value representing the container farm's ID. </param>
        /// <returns> Returns a container farm object or null dependant on if the farm exists with the given ID. </returns>
        /// <exception cref="ArgumentException"> If there are any issues communicating with the database an error is thrown. </exception>
        public ContainerFarm GetContainerFarmById(int id)
        {
            try
            {
                return _connection.GetAllWithChildren<ContainerFarm>().FirstOrDefault(containerFarm => containerFarm.Id == id);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        /// <summary>
        /// Method to be called when a user wants to delete a specific container farm from the database.
        /// </summary>
        /// <param name="containerFarm"> Takes in any container farm field. </param>
        /// <exception cref="ArgumentException"> Throws an exception if there is an issue deleting a specific container farm from the database. </exception>
        public void DeleteContainerFarm(ContainerFarm containerFarm)
        {
            try
            {
                _connection.Delete<ContainerFarm>(containerFarm.Id);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
} 