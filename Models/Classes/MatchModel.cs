﻿using Models.Enums;
using System;

namespace Models.Classes
{
    public class MatchModel
    {
        public int ID { get; set; }

        public string Winner { get; set; }

        public string Loser { get; set; }

        public string League { get; set; }

        public LeagueTypesEnum LeagueType { get; set; }

        public DateTime DateTime { get; set; }
    }
}
