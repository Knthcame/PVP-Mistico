﻿using Models.Enums;
using Prism.Commands;
using Prism.Navigation;
using PVPMistico.Dictionaries;
using PVPMistico.Logging.Interfaces;
using PVPMistico.Managers.Interfaces;
using PVPMistico.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PVPMistico.ViewModels
{
    public class CreateTournamentPopUpVIewModel : BaseViewModel
    {
        private readonly ITournamentManager _tournamentManager;

        public List<LeagueTypePickerItemModel> LeagueTypes { get; set; } = CreateLeagueTypesList();

        public string LeagueName { get; set; }

        public LeagueTypePickerItemModel SelectedLeagueType { get; set; }

        public ICommand CreateTournamentCommand { get; private set; }

        public CreateTournamentPopUpVIewModel(INavigationService navigationService, ICustomLogger logger, ITournamentManager tournamentManager)
            : base(navigationService, logger)
        {
            _tournamentManager = tournamentManager;
            CreateTournamentCommand = new DelegateCommand(async() => await OnCreateTournamentButtonPressedAsync());
        }

        private static List<LeagueTypePickerItemModel> CreateLeagueTypesList()
        {
            var enums = Enum.GetValues(typeof(LeagueTypesEnum));
            var list = new List<LeagueTypePickerItemModel>();
            foreach (LeagueTypesEnum leagueTypeEnum in enums)
            {
                if (LeagueTypesDictionary.GetLeagueTypeString(leagueTypeEnum, out string leagueTypeName))
                    list.Add(new LeagueTypePickerItemModel(leagueTypeEnum, leagueTypeName));
            }

            return list;
        }

        private async Task OnCreateTournamentButtonPressedAsync()
        {
            _tournamentManager.CreateTournament(LeagueName, SelectedLeagueType.LeagueTypesEnum);
            await NavigationService.ClearPopupStackAsync();
        }

        public override bool OnBackButtonPressed()
        {
            NavigationService.ClearPopupStackAsync();
            return true;
        }
    }
}
