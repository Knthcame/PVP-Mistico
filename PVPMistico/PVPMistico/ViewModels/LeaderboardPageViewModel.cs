﻿using Acr.UserDialogs;
using Models.Classes;
using Prism.Commands;
using Prism.Navigation;
using PVPMistico.Constants;
using PVPMistico.Enums;
using PVPMistico.Logging.Interfaces;
using PVPMistico.Managers.Interfaces;
using PVPMistico.Resources;
using PVPMistico.Views;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace PVPMistico.ViewModels
{
    public class LeaderboardPageViewModel : BaseViewModel
    {
        private readonly ITournamentManager _tournamentManager;
        private readonly IDialogManager _dialogManager;
        private LeaderboardModel _leaderboard;
        private bool _isCurrentUserAdmin;

        public LeaderboardModel Leaderboard
        {
            get => _leaderboard;
            set => SetProperty(ref _leaderboard, value);
        }

        public bool IsCurrentUserAdmin
        {
            get => _isCurrentUserAdmin;
            set => SetProperty(ref _isCurrentUserAdmin, value);
        }

        public ICommand AddTrainerCommand { get; private set; }

        public LeaderboardPageViewModel(INavigationService navigationService, ICustomLogger logger, ITournamentManager tournamentManager, IDialogManager dialogManager)
            : base(navigationService, logger)
        {
            _tournamentManager = tournamentManager;
            _dialogManager = dialogManager;
            AddTrainerCommand = new DelegateCommand(async () => await OnAddTrainerButtonPressedAsync());
        }

        private async Task OnAddTrainerButtonPressedAsync()
        {
            var parameters = new NavigationParameters()
            {
                {NavigationParameterKeys.LeaderboardKey, Leaderboard }
            };
            await NavigationService.NavigateAsync(nameof(AddTrainerPopup), parameters);
        }

        public override async void OnNavigatingTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (parameters.TryGetValue(NavigationParameterKeys.LeaderboardIdKey, out int id) && Leaderboard != null)
                Leaderboard = _tournamentManager.GetLeaderboard(id);
            else
            {
                await NavigationService.GoBackAsync();
                _dialogManager.ShowToast(new ToastConfig(AppResources.LeaderboardNotFoundToast), ToastModes.Error);
                return;
            }

            var usernameTask = SecureStorage.GetAsync(SecureStorageTokens.Username);
            var username = usernameTask.Result;

            var currentUser = Leaderboard.Participants.FirstOrDefault((trainer) => trainer.Username == username);
            if (currentUser != null && currentUser.IsAdmin == true)
                IsCurrentUserAdmin = true;

        }
    }
}
