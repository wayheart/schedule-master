using Newtonsoft.Json;

namespace Slax.Schedule
{
    [JsonObject]
    public struct SerializableDateTime
    {
        public int Date;
        public int Year;
        public int Hour;
        public int Minutes;
        public Season Season;
        public Month Month;
        public DayConfiguration DayConfiguration;
    }
}