﻿using System.Threading.Tasks;
using PVPMistico.Constants;
using PVPMistico.Managers.Interfaces;
using Xamarin.Essentials;

namespace PVPMistico.Managers
{
    public class AccountManager : IAccountManager
    {
        IHttpManager HttpManager;
        public AccountManager(IHttpManager httpManager)
        {
            HttpManager = httpManager;
        }

        public async Task<string> LogInAsync(string username, string password)
        {
            if (!username.Equals("Originals"))
            {
                return LogInResponses.UsernameNotFound;
            }
            else if (!password.Equals("Test123"))
            {
                return LogInResponses.PasswordIncorrect;
            }
            else
            {
                await SecureStorage.SetAsync(SecureStorageTokens.Username, username);
                return LogInResponses.LogInSuccesfull;
            }
        }

        public bool CheckUsernameRegistered(string username)
        {
            return username == "Originals";
        }

        public async Task<string> SignInAsync(string name, string email, string username, string password)
        {
            if(username == "Originals")
            {
                return SignInResponses.UserAlreadyRegistered;
            }
            else
            {
                await SecureStorage.SetAsync(SecureStorageTokens.Username, username);
                await SecureStorage.SetAsync(SecureStorageTokens.Name, name);
                await SecureStorage.SetAsync(SecureStorageTokens.Email, email);

                return SignInResponses.SignInSuccessful;
            }
        }

        public void LogOut()
        {
            SecureStorage.Remove(SecureStorageTokens.Username);
            SecureStorage.Remove(SecureStorageTokens.Email);
            SecureStorage.Remove(SecureStorageTokens.Name);
        }
    }
}
