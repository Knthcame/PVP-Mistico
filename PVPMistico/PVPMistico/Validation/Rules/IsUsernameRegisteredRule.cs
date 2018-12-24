﻿using PVPMistico.Managers.Interfaces;
using PVPMistico.Resources;
using PVPMistico.Validation.Rules.Interfaces;

namespace PVPMistico.Validation.Rules
{
    public class IsUsernameRegisteredRule : IValidationRule<string>
    {
        public string ValidationMessage { get; set; } = AppResources.UserNotRegisteredError;
        private IAccountManager AccountManager { get; set; }

        public IsUsernameRegisteredRule(IAccountManager accountManager)
        {
            AccountManager = accountManager;
        }
        public bool Check(string username)
        {
            return AccountManager.CheckUsernameRegistered(username);
        }
    }
}
