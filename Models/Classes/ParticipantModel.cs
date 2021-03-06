﻿namespace Models.Classes
{
    public class ParticipantModel
    {
        public string Username { get; set; }

        public int Level { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Points { get; set; } = 500;

        public int Position { get; set; }

        public bool IsAdmin { get; set; }

        public ParticipantModel() { }
    }
}
