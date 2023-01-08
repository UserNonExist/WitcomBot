using Discord;
using Discord.WebSocket;
using WitcomBot.Database;

namespace WitcomBot;

public class CommandHandlers
{
    public string botVersion { get; set; }= "1.1.2";
    
    public async Task AddPingTrigger(SocketSlashCommand command)
    {
        string UserId = command.User.Id.ToString();
        string? Message = command.Data.Options.FirstOrDefault().Value.ToString();
        string Title = "Set Ping Trigger";
        
        if (Message.Length > 50)
        {
            await BuildMessageEmbed(Color.Red, Title, "ข้อความต้องน้อยกว่า 50 ตัว", true, command);
            return;
        }
        
        Extensions.UpdateMessage(UserId, Message);
        
        await BuildMessageEmbed(Color.Green, Title, "บอทได้ตั้งค่าข้อความคุณสำเร็จแล้ว!", true, command);
    }
    public async Task DisablingPingTrigger(SocketSlashCommand command)
    {
        string UserId = command.User.Id.ToString();
        string? disable = command.Data.Options.FirstOrDefault().Value.ToString();
        string Title = "Toggle Ping Trigger";
        
        Extensions.TriggerDisabled(UserId, disable);
        
        if (disable == "False")
            await BuildMessageEmbed(Color.Green, Title, "บอทได้ตั้งค่าข้อความคุณสำเร็จแล้ว!", true, command);
        if (disable == "True")
            await BuildMessageEmbed(Color.Green, Title, "คุณได้หยุดบอทให้ส่งข้อความไปหาคนอื่นแล้ว", true, command);
    }
    public async Task GetPingTrigger(SocketSlashCommand command)
    {
        string UserId = command.User.Id.ToString();
        string? Message = Extensions.GetMessage(UserId);
        string Title = "Get Ping Trigger";
        
        if (Message == null)
        {
            await BuildMessageEmbed(Color.Red, Title, "คุณไม่มีข้อความที่ตั้งไว้", true, command);
            return;
        }

        await BuildMessageEmbed(Color.Green, Title, $"ข้อความที่คุณตั้งไว้: {Message}", true, command);
    }
    public async Task DeletePingTrigger(SocketSlashCommand command)
    {
        string UserId = command.User.Id.ToString();
        string Message = Extensions.GetMessage(UserId);
        string Title = "Delete Ping Trigger";
        
        if (Message == null)
        {
            await BuildMessageEmbed(Color.Red, Title, "คุณไม่มีข้อความที่ตั้งไว้", true, command);
            return;
        }
        
        Extensions.DeleteMessage(UserId);
        
        await BuildMessageEmbed(Color.Green, Title, "บอทได้ลบข้อความของคุณออกแล้ว", true, command);
    }
    public async Task GetInfo(SocketSlashCommand command)
    {
        await BuildMessageEmbed(Color.Blue, "Info", $"เวอร์ชั่นบอท: {botVersion}\n\nผู้พัฒนา: {MentionUtils.MentionUser(315717809395204098)}\n\nGithub Repo: https://github.com/UserNonExist/WitcomBot/", false, command);
    }
    public async Task CurseTae(SocketSlashCommand command)
    {
        string Title = "ด่าไอ้เต้";
        string FilePath = "./Data/CurseMessage.txt";

        if (!File.Exists(FilePath))
        {
            File.Create(FilePath).Close();
        }
        
        
        List<string> cursedMessage = File.ReadAllLines(FilePath).ToList();
        
        if (cursedMessage.Count == 0)
        {
            await BuildMessageEmbed(Color.Red, Title, "ไม่มีข้อความที่ถูกเพิ่มเข้ามา", true, command);
            return;
        }
        
        Random random = new();
        int randomIndex = random.Next(0, cursedMessage.Count);
        
        await command.Channel.SendMessageAsync(MentionUtils.MentionUser(723069900746129491));
        await BuildMessageEmbed(Color.Green, Title, cursedMessage[randomIndex], false, command);
        
    }

    public async Task AddCurse(SocketSlashCommand command)
    {
        string Title = "เพิ่มข้อความด่า";
        string FilePath = "./Data/CurseMessage.txt";
        
        if (!File.Exists(FilePath))
        {
            File.Create(FilePath).Close();
        }
        
        string Message = command.Data.Options.FirstOrDefault().Value.ToString();
        
        List<string> cursedMessage = File.ReadAllLines(FilePath).ToList();
        cursedMessage.Add(Message);
        File.WriteAllLines(FilePath, cursedMessage);
        
        await BuildMessageEmbed(Color.Green, Title, "บอทได้เพิ่มข้อความด่าเข้าไปแล้ว", true, command);
    }
    
    
    
    // Embed Builder
    private async Task BuildMessageEmbed(Color colour, string title, string content, bool epher, SocketSlashCommand command)
    {
        var embed = new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(content)
            .WithColor(colour)
            .WithCurrentTimestamp()
            .WithFooter($"WITCOM Utilities Bot ~ สร้างโดย User_NotExist#0529 ~ เวอร์ชั่น {botVersion}");
        
        await command.RespondAsync(embed: embed.Build(), ephemeral: epher);
    }
}