using Discord;
using Discord.WebSocket;
using WitcomBot.Database;

namespace WitcomBot;

public class EventHandlers
{
    public string botVersion { get; set; } = "1.1.2";
    
    public async Task MessageRecieved(SocketMessage message)
    {
        try
        {
            if (message.Author.IsBot)
                return;
            if (message.MentionedUsers.Count > 0)
                PingTriggered(message);
            if (message.MentionedEveryone)
                PingEveryoneTriggered(message);

            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task PingTriggered(SocketMessage message)
    {
        ulong mentionable = message.MentionedUsers.First().Id;
            
        string mention = mentionable.ToString();

        if (mention == message.Author.Id.ToString())
            return;

        if (Extensions.GetMessage(mention) == null)
            return;
            
        var pingTrigger = Extensions.GetMessage(mention);
        var disabled = Extensions.GetDisabled(mention);

        if (disabled != "True")
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("Ping Trigger")
                .WithDescription( $"{MentionUtils.MentionUser(message.Author.Id)} - {pingTrigger}")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter($"WITCOM Utilities Bot ~ สร้างโดย User_NotExist#0529 ~ เวอร์ชั่น {botVersion}");

            await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
    
    private async Task PingEveryoneTriggered(SocketMessage message)
    {
        var embedBuilder = new EmbedBuilder()
                .WithTitle("Ping Trigger")
                .WithDescription( $"{MentionUtils.MentionUser(message.Author.Id)} - แท็กทุกคนหา พ่ อ ง")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter($"WITCOM Utilities Bot ~ สร้างโดย User_NotExist#0529 ~ เวอร์ชั่น {botVersion}");

            await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
    }
}