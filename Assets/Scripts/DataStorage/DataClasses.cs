using System.Collections.Generic;

namespace DataStorage
{
    public abstract class DataClasses
    {
        public class ScoringEvents
        {
            public List<string> ScoringEventsJsonList;

            public class ScoringEvent
            {
                public string TimeStamp;
                public int Score;
            }
        }
    }
}