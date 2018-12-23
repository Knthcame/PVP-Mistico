﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Prism.Commands;
using Prism.Navigation;
using PVPMistico.Constants;
using PVPMistico.Managers.Interfaces;
using PVPMistico.Validation.Rules;
using PVPMistico.Views;

namespace PVPMistico.ViewModels
{
    public class LogInPageViewModel : BaseAccountValidationViewModel
    {
        #region Properties
        public DelegateCommand LogInCommand { get; private set; }
        public DelegateCommand SignInCommand { get; private set; }
        #endregion

        public LogInPageViewModel(INavigationService navigationService, IAccountManager accountManager) : base(navigationService, accountManager)
        {
            Title = "Inicia sesión";

            LogInCommand = new DelegateCommand(async () => await OnLogInButtonPressed());
            SignInCommand = new DelegateCommand(async () => await OnSignInButtonPressed());
        }

        protected override void AddValidations()
        {
            base.AddValidations();
            Username.Validations.Add(new IsUsernameRegisteredRule(AccountManager));
        }

        private async Task OnSignInButtonPressed()
        {
            try
            {
                await NavigationService.NavigateAsync(nameof(SignInPage));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private async Task OnLogInButtonPressed()
        {
            CheckCredentials();
            if (!AreCredentialsValid)
                return;

            if (AccountManager.LogIn(Username.Value, Password.Value, out string logInResponse))
                await NavigationService.NavigateAsync("/NavigationPage/" + nameof(MainPage));
            else
            {
                var toastConfig = new ToastConfig(logInResponse);
                toastConfig.SetPosition(ToastPosition.Bottom);
                UserDialogs.Instance.Toast(toastConfig);

                var errors = new List<string>
                {
                    logInResponse
                };

                switch (logInResponse)
                {
                    case LogInResponses.UsernameNotFound:
                        Username.Errors = errors;
                        Username.IsValid = false;
                        break;

                    case LogInResponses.PasswordIncorrect:
                        Password.Errors = errors;
                        Password.IsValid = false;
                        break;
                }
            }
        }
    }
}
