﻿using Models.Enums;

namespace Models.Classes
{
    public class LeaderBoardPreviewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ParticipantModel Participant { get; set; }

        public LeagueTypesEnum LeagueType { get; set; }
    }
}
