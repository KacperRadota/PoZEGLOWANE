using System;
using System.Collections.Generic;

namespace DataStorage
{
    public abstract class DataClasses
    {
        [Serializable]
        public class Boats
        {
            public List<Boat> boatsList;
            public Boat currentlyChosenBoat;

            [Serializable]
            public class Boat
            {
                public ulong id;
                public string boatName;
                public string checkInDay;
                public string checkInMonth;
                public string checkInYear;
                public string checkOutDay;
                public string checkOutMonth;
                public string checkOutYear;
                public List<string> crewMembersNames;
                public string notes;
                public List<ScoringEvent> scoringEvents;
                public int lastCalculatedScore;
                public int lastCalculatedScoreListCount;

                [Serializable]
                public class ScoringEvent
                {
                    public string timeStamp;
                    public int score;
                }
            }
        }
    }
}