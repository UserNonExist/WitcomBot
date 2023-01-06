
using Discord.Net;

namespace WitcomBot.Database;

public class Extensions
{
    private static void AddUser(string user = "", PingTrigger? pingTrigger = null)
    {
        try
        {
            if (!Database.LiteDatabase.GetCollection<PingTrigger>().Exists(x => x.UserId == user))
                Database.LiteDatabase.GetCollection<PingTrigger>().Insert(pingTrigger);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void UpdateMessage(string user = "", string? message = "")
    {
        try
        {
            if (!Database.LiteDatabase.GetCollection<PingTrigger>().Exists(x => x.UserId == user))
            {
                AddUser(user, new PingTrigger
                {
                    disable = "False",
                    Message = message,
                    UserId = user
                });
                return;
            }
            
            var mTrigger = Database.LiteDatabase.GetCollection<PingTrigger>().FindOne(x => x.UserId == user);
            mTrigger.Message = message;
            
            Database.LiteDatabase.GetCollection<PingTrigger>().Update(mTrigger);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void TriggerDisabled(string user, string? disabling)
    {
        try
        {
            if (!Database.LiteDatabase.GetCollection<PingTrigger>().Exists(x => x.UserId == user))
            {
                AddUser(user, new PingTrigger
                {
                    disable = disabling,
                    Message = "",
                    UserId = user,
                });
                return;
            }
            
            var mTrigger = Database.LiteDatabase.GetCollection<PingTrigger>().FindOne(x => x.UserId == user);

            mTrigger.disable = disabling;
            
            Database.LiteDatabase.GetCollection<PingTrigger>().Update(mTrigger);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void DeleteMessage(string user)
    {
        try
        {
            if (!Database.LiteDatabase.GetCollection<PingTrigger>().Exists(x => x.UserId == user))
            {
                AddUser(user, new PingTrigger
                {
                    disable = "False",
                    Message = "",
                    UserId = user
                });
            }
            
            var mTrigger = Database.LiteDatabase.GetCollection<PingTrigger>().FindOne(x => x.UserId == user);
            mTrigger.Message = "";
            
            Database.LiteDatabase.GetCollection<PingTrigger>().Update(mTrigger);
        }
        catch(HttpException exception)
        {
            Console.WriteLine(exception);
        }
    }

    public static string? GetMessage(string? user = null)
        => !Database.LiteDatabase.GetCollection<PingTrigger>().Exists(x => x.UserId == user)
            ? null : Database.LiteDatabase.GetCollection<PingTrigger>().FindOne(x => x.UserId == user).Message;
    
    public static string? GetDisabled(string? user = null)
        => !Database.LiteDatabase.GetCollection<PingTrigger>().Exists(x => x.UserId == user)
            ? null : Database.LiteDatabase.GetCollection<PingTrigger>().FindOne(x => x.UserId == user).disable;
}