
using LiteDB;

namespace WitcomBot.Database;


public class PingTrigger
    {
        [BsonId]
        public string UserId { get; set; }
        public string? Message { get; set; }
        public string? disable { get; set; }
    }