﻿using Models.Classes;
using Models.Enums;
using Models.Crypto;

namespace PVPService.Services
{
    public class AccountsRepository
    {
        private Database _database = new Database();

        public bool IsAccountRegistered(string username)
        {
            if (username == null)
                return false;

            return _database.GetAccounts().Find((account) => account.Username == username) != null;
        }

        public bool IsEmailRegistered(string email)
        {
            if (email == null)
                return false;

            return _database.GetAccounts().Find((account) => account.Email == email) != null;
        }

        public SignInResponseCode RegisterNewAccount(AccountModel account)
        {
            if (account == null)
                return SignInResponseCode.UnknowError;

            else if (IsAccountRegistered(account.Username))
                return SignInResponseCode.UsernameAlreadyRegistered;

            else if (IsEmailRegistered(account.Email))
                return SignInResponseCode.EmailAlreadyUsed;
            
            var added =_database.AddAccount(account);

            if (added)
                return SignInResponseCode.SignInSuccessful;
            else
                return SignInResponseCode.UnknowError;
        }

        public LogInResponseCode ValidateCredentials(AccountModel account)
        {
            if (!IsAccountRegistered(account.Username))
                return LogInResponseCode.UsernameNotRegistered;

            var registeredAccount = _database.GetAccounts().Find((user) => user.Username == account.Username);
            if (CompareEncryptedPasswords(account.Password, registeredAccount.Password))
                return LogInResponseCode.LogInSuccessful;
            else
                return LogInResponseCode.PasswordIncorrect;

        }

        private bool CompareEncryptedPasswords(string password1, string password2)
        {
            string key = "Originals rule";
            return password1.Decrypt(key).Equals(password2.Decrypt(key));
        }
    }
}
