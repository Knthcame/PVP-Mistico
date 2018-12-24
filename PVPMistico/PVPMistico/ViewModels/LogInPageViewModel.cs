﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;
using PVPMistico.Constants;
using PVPMistico.Logging.Interfaces;
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

        public LogInPageViewModel(INavigationService navigationService, IAccountManager accountManager, IDialogManager dialogManager, ICustomLogger logger) 
            : base(navigationService, accountManager, dialogManager, logger)
        {
            Title = "Inicia sesión";

            LogInCommand = new DelegateCommand(async () => await OnLogInButtonPressed());
            SignInCommand = new DelegateCommand(async () => await OnSignInButtonPressed());
        }

        protected override void AddValidations()
        {
            base.AddValidations();
            Username.Validations.Add(new IsUsernameRegisteredRule(_accountManager));
        }

        private async Task OnSignInButtonPressed()
        {
            try
            {
                await NavigationService.NavigateAsync(nameof(SignInPage));
            }
            catch (Exception e)
            {
                _logger.Log(e.Message, e, Prism.Logging.Category.Exception, Prism.Logging.Priority.High);
            }
        }

        private async Task OnLogInButtonPressed()
        {
            CheckCredentials();
            if (!AreCredentialsValid)
                return;

            var logInResponse = await _accountManager.LogInAsync(Username.Value, Password.Value);

            if (logInResponse == LogInResponses.LogInSuccesfull)
                await NavigationService.NavigateAsync("/NavigationPage/" + nameof(MainPage));
            else
            {
                _dialogManager.ShowToast(logInResponse);

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
