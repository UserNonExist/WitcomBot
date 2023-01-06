using LiteDB;

namespace WitcomBot.Database;

public class Database
{
    public static LiteDatabase LiteDatabase { get; set; }
    public static string Folder = "./Data/";
    public static string FileName = Path.Combine(Folder, "PingTrigger.db");

    public static void Initialize()
    {
        if (!Directory.Exists(Folder))
        {
            Directory.CreateDirectory(Folder);
        }

        LiteDatabase = new LiteDatabase(new ConnectionString(FileName)
        {
            Connection = ConnectionType.Shared
        });

        LiteDatabase.GetCollection<PingTrigger>().EnsureIndex(x => x.UserId, true);
        LiteDatabase.GetCollection<PingTrigger>().EnsureIndex(x => x.Message);
        LiteDatabase.GetCollection<PingTrigger>().EnsureIndex(x => x.disable);
    }

    public static void Close()
    {
        LiteDatabase.Checkpoint();
        LiteDatabase.Dispose();
        LiteDatabase = null;
    }
}
