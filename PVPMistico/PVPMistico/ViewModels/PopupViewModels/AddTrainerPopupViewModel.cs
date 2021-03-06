﻿using Acr.UserDialogs;
using Models.Classes;
using Models.Enums;
using Prism.Commands;
using Prism.Navigation;
using PVPMistico.Constants;
using PVPMistico.Enums;
using PVPMistico.Extensions;
using PVPMistico.Logging.Interfaces;
using PVPMistico.Managers.Interfaces;
using PVPMistico.Resources;
using PVPMistico.ViewModels.BaseViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVPMistico.ViewModels.PopupViewModels
{
    public class AddTrainerPopupViewModel : BaseViewModel
    {
        private readonly IAccountManager _accountManager;
        private readonly ILeaderboardManager _leaderboardManager;
        private readonly IDialogManager _dialogManager;
        private LeaderboardModel _leaderboard;
        private ObservableCollection<TrainerModel> _trainerList;
        private TrainerModel _selectedTrainer;

        public ObservableCollection<TrainerModel> TrainerList
        {
            get => _trainerList;
            set => SetProperty(ref _trainerList, value);
        }

        public TrainerModel SelectedTrainer
        {
            get => _selectedTrainer;
            set => SetProperty(ref _selectedTrainer, value);
        }

        public ICommand SearchTrainerCommand { get; private set; }
        public ICommand TrainerSelectedCommand { get; private set; }
        public string SearchText { get; set; }

        public AddTrainerPopupViewModel(INavigationService navigationService, ICustomLogger logger, IAccountManager accountManager, ILeaderboardManager leaderboardManager, IDialogManager dialogManager)
            : base(navigationService, logger)
        {
            _accountManager = accountManager;
            _leaderboardManager = leaderboardManager;
            _dialogManager = dialogManager;

            IsPageLoading = true;
            SearchTrainerCommand = new DelegateCommand(async () => await OnTrainerSearch());
            TrainerSelectedCommand = new DelegateCommand(async () => await OnTrainerSelectedAsync());
        }

        private async Task OnTrainerSelectedAsync()
        {
            _dialogManager.ShowLoading(AppResources.Loading);
            var response = await _leaderboardManager.AddTrainerAsync(_leaderboard, SelectedTrainer);

            _dialogManager.EndLoading();
            switch (response)
            {
                case AddTrainerResponseCode.TrainerAddedSuccesfully:
                    await NavigationService.GoBackAsync();
                    return;

                case AddTrainerResponseCode.TrainerAlreadyParticipates:
                    var config = new ToastConfig(AppResources.CannotAddTrainer);
                    _dialogManager.ShowToast(config, ToastModes.Error);

                    SelectedTrainer = null;
                    break;

                default:
                    _dialogManager.ShowToast(new ToastConfig(AppResources.Error), ToastModes.Error);
                    SelectedTrainer = null;
                    break;
            }
        }

        private async Task OnTrainerSearch()
        {
            if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText))
                await SetFullTrainerListAsync();
            else
                TrainerList = new ObservableCollection<TrainerModel>(TrainerList.Where((trainer) => trainer.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        private async Task SetFullTrainerListAsync()
        {
            var trainers = await _accountManager.GetRegisteredTrainers();
            RemoveAlreadyParticipatingTrainers(trainers);
            TrainerList = new ObservableCollection<TrainerModel>(trainers);
        }

        private void RemoveAlreadyParticipatingTrainers(IList<TrainerModel> trainers)
        {
            if (_leaderboard == null)
                return;

            foreach (ParticipantModel participant in _leaderboard.Participants)
            {
                var trainerToRemove = trainers.FirstOrDefault((trainer) => trainer.Username == participant.Username);
                trainers.Remove(trainerToRemove);
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);

            if (!parameters.TryGetValue(NavigationParameterKeys.LeaderboardKey, out _leaderboard))
            {
                var config = new ToastConfig(AppResources.Error);
                _dialogManager.ShowToast(config, ToastModes.Error);
                await NavigationService.GoBackAsync();
            }
            else
            {
                await SetFullTrainerListAsync();
                IsPageLoading = false;
                if (TrainerList == null || TrainerList.Count == 0)
                {
                    var config = new AlertConfig()
                    {
                        Message = AppResources.NoTrainersLeftToAdd,
                        OkText = AppResources.AlertDialogOkButton,
                        OnAction = async () => await NavigationService.GoBackAsync()
                    };
                    _dialogManager.ShowAlert(config);
                }
            }
        }
    }
}
