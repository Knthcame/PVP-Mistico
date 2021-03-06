﻿using Acr.UserDialogs;
using Models.Classes;
using Prism.Commands;
using Prism.Navigation;
using PVPMistico.Constants;
using PVPMistico.Logging.Interfaces;
using PVPMistico.Managers.Interfaces;
using PVPMistico.Resources;
using PVPMistico.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using PVPMistico.Views.Popups;
using PVPMistico.ViewModels.BaseViewModels;

namespace PVPMistico.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private string _menuText;
        private LeaderBoardPreviewModel _selectedLeaderboard;
        private ObservableCollection<LeaderBoardPreviewModel> _leaderboardPreviews;
        private bool _isLeaderboardPreviewListRefreshing;
        private bool _isLeaderboardPreviewListEmpty;
        private readonly ILeaderboardManager _leaderboardManager;
        private readonly IAccountManager _accountManager;
        private readonly IDialogManager _dialogManager;

        public string MenuText
        {
            get => _menuText;
            set => SetProperty(ref _menuText, value);
        }

        public LeaderBoardPreviewModel SelectedLeaderboard
        {
            get => _selectedLeaderboard;
            set => SetProperty(ref _selectedLeaderboard, value);
        }

        public bool IsLeaderboardPreviewListRefreshing
        {
            get => _isLeaderboardPreviewListRefreshing;
            set => SetProperty(ref _isLeaderboardPreviewListRefreshing, value);
        }

        public bool IsLeaderboardPreviewListEmpty
        {
            get => _isLeaderboardPreviewListEmpty;
            set => SetProperty(ref _isLeaderboardPreviewListEmpty, value);
        }

        public ObservableCollection<LeaderBoardPreviewModel> LeaderboardPreviews
        {
            get => _leaderboardPreviews;
            set => SetProperty(ref _leaderboardPreviews, value);
        }

        public ICommand MenuItemCommand { get; private set; }
        public ICommand CreateTournamentCommand { get; private set; }
        public ICommand SelectedLeaderboardCommand { get; private set; }
        public ICommand ReloadEmptyPreviewsCommand { get; private set; }
        public ICommand ResfreshLeaderboardPreviewsCommand { get; private set; }

        public MainPageViewModel(INavigationService navigationService, IAccountManager accountManager, ICustomLogger logger, IDialogManager dialogManager, ILeaderboardManager leaderboardManager)
            : base(navigationService, logger)
        {
            _accountManager = accountManager;
            _dialogManager = dialogManager;
            _leaderboardManager = leaderboardManager;

            Title = AppResources.MainPageTitle;
            MenuText = AppResources.LogOutButtonText;
            MenuItemCommand = new DelegateCommand(OnLogOutClicked);
            CreateTournamentCommand = new DelegateCommand(async () => await OnCreateTournamentButtonClickedAsync());
            SelectedLeaderboardCommand = new DelegateCommand(async () => await OnLeaderboardSelectedAsync());
            ReloadEmptyPreviewsCommand = new DelegateCommand(async () => await ReloadEmptyPreviews());
            ResfreshLeaderboardPreviewsCommand = new DelegateCommand(async () => await RefreshLeadeboardPreviews());
        }

        private async Task OnCreateTournamentButtonClickedAsync()
        {
            await NavigationService.NavigateAsync(nameof(CreateLeaderboardPopup));
        }

        private async Task OnLeaderboardSelectedAsync()
        {
            if (SelectedLeaderboard == null)
                return;

            var parameters = new NavigationParameters
            {
                { NavigationParameterKeys.LeaderboardIdKey, SelectedLeaderboard.ID },
                { NavigationParameterKeys.LeaderboardNameKey, SelectedLeaderboard.Name }
            };
            await NavigationService.NavigateAsync(nameof(LeaderboardPage), parameters);

            SelectedLeaderboard = null;
        }

        private async Task ReloadEmptyPreviews()
        {
            IsLeaderboardPreviewListEmpty = false;
            IsPageLoading = true;
            await LoadLeadeboardPreviews();
            IsPageLoading = false;
        }

        private async Task RefreshLeadeboardPreviews()
        {
            IsLeaderboardPreviewListRefreshing = true;
            await LoadLeadeboardPreviews();
            IsLeaderboardPreviewListRefreshing = false;
        }

        private async Task LoadLeadeboardPreviews()
        {
            var username = await SecureStorage.GetAsync(SecureStorageTokens.Username);
            var leaderboards = await _leaderboardManager.GetMyLeaderboardsAsync(username);

            var leaderboardPreviews = new ObservableCollection<LeaderBoardPreviewModel>();

            if (leaderboards != null)
            {
                foreach (LeaderboardModel leaderboard in leaderboards)
                {
                    var leaderboardPreview = new LeaderBoardPreviewModel()
                    {
                        ID = leaderboard.ID,
                        LeagueType = leaderboard.LeagueType,
                        Name = leaderboard.Name,
                        Participant = leaderboard.Participants.FirstOrDefault((participant) => participant.Username == username)
                    };
                    leaderboardPreviews.Add(leaderboardPreview);
                }
            }
            IsLeaderboardPreviewListEmpty = leaderboardPreviews.Count == 0;
            LeaderboardPreviews = leaderboardPreviews;
            IsLeaderboardPreviewListRefreshing = false;
        }

        private void OnLogOutClicked()
        {
            var config = new ConfirmConfig()
            {
                Title = AppResources.LogOutConfirmationTitle,
                Message = AppResources.LogOutConfirmationMessage,
                OkText = AppResources.ConfirmationDialogOkButton,
                CancelText = AppResources.ConfirmationDialogCancelButton,
                OnAction = OnLogOutAction
            };
            _dialogManager.ShowConfirmationDialog(config);
        }

        private async void OnLogOutAction(bool confirmed)
        {
            if (confirmed)
            {
                _accountManager.LogOut();
                await NavigationService.NavigateAsync("/NavigationPage/" + nameof(LogInPage));
            }
        }

        public override async void OnNavigatingTo(INavigationParameters parameters)
        {
            IsPageLoading = true;
            base.OnNavigatedTo(parameters);

            await LoadLeadeboardPreviews();
            IsPageLoading = false;
        }
    }
}
