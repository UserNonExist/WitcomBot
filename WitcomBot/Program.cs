// See https://aka.ms/new-console-template for more information

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace WitcomBot;


public class Program
{

    public static Task Main(string[] args) => new Program().MainAsync();
    
    private DiscordSocketClient _client;
    public CommandHandlers CommandHandlers;
    public EventHandlers EventHandlers;

    private static string textFolder = "./Token/";

    public async Task MainAsync()
    {
        try
        {

            _client = new DiscordSocketClient();
            _client.Log += Log;
            Database.Database.Initialize();

            if (!Directory.Exists(textFolder))
            {
                Directory.CreateDirectory(textFolder);
                File.WriteAllText("./Token/token.txt", "Your Token Here");

                Console.WriteLine("Please enter your token in the token.txt file");
                await Task.Delay(-1);
            }

            var token = File.ReadAllText("./Token/token.txt");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            CommandHandlers = new CommandHandlers();
            EventHandlers = new EventHandlers();

            //Hooking up the event
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += command => SlashCommandHandler(command);
            _client.MessageReceived += EventHandlers.MessageRecieved;

            // Block this task until the program is closed.
            await Task.Delay(-1);
            _client.Ready -= Client_Ready;
            _client.SlashCommandExecuted -= command => SlashCommandHandler(command);
            _client.MessageReceived -= EventHandlers.MessageRecieved;
            Database.Database.Close();
        }
        catch (GatewayReconnectException e)
        {
            await _client.LogoutAsync();
            await Task.Delay(5000);
            await _client.LoginAsync(TokenType.Bot, "./Token/token.txt");
            await _client.StartAsync();
            throw;
        }
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
    
    //The unholy begins
    //This is the event that will be called when the bot is ready
    private async Task Client_Ready()
    {
        Console.WriteLine("Creating commands");
        List<ApplicationCommandProperties> applicationCommandProperties = new();
        
        SlashCommandBuilder globalCommandPtUpdate = new SlashCommandBuilder();
        globalCommandPtUpdate.WithName("pt-message");
        globalCommandPtUpdate.WithDescription("สร้างหรือเปลื่ยนข้อความที่จะตอบกับคนที่ปิงคุณ");
        globalCommandPtUpdate.AddOption("message", ApplicationCommandOptionType.String, "ข้อความที่จะส่ง", isRequired: true);
        applicationCommandProperties.Add(globalCommandPtUpdate.Build());

        SlashCommandBuilder globalCommandPtDisable = new SlashCommandBuilder();
        globalCommandPtDisable.WithName("pt-disable");
        globalCommandPtDisable.WithDescription("ปิดการตอบกลับข้อความที่จะตอบกับคนที่ปิงคุณ");
        globalCommandPtDisable.AddOption("is_disabled", ApplicationCommandOptionType.Boolean, "ปิดการตอบกลับไหม", isRequired: true);
        applicationCommandProperties.Add(globalCommandPtDisable.Build());
        
        SlashCommandBuilder globalCommandPtGet = new SlashCommandBuilder();
        globalCommandPtGet.WithName("pt-get");
        globalCommandPtGet.WithDescription("ดูข้อความที่จะตอบกับคนที่ปิงคุณ");
        applicationCommandProperties.Add(globalCommandPtGet.Build());
        
        SlashCommandBuilder globalCommandPtDelete = new SlashCommandBuilder();
        globalCommandPtDelete.WithName("pt-delete");
        globalCommandPtDelete.WithDescription("ลบข้อความที่จะตอบกับคนที่ปิงคุณ");
        applicationCommandProperties.Add(globalCommandPtDelete.Build());
        
        SlashCommandBuilder globalCommandInfo = new SlashCommandBuilder();
        globalCommandInfo.WithName("info");
        globalCommandInfo.WithDescription("ดูข้อมูลของบอท");
        applicationCommandProperties.Add(globalCommandInfo.Build());
        
        try
        {
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
            Console.WriteLine("Created global command");
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            
            Console.WriteLine(json);
        }
    }
    
    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "pt-message":
                await CommandHandlers.AddPingTrigger(command);
                break;
            
            case "pt-disable":
                await CommandHandlers.DisablingPingTrigger(command);
                break;
            
            case "pt-get":
                await CommandHandlers.GetPingTrigger(command);
                break;
            
            case "pt-delete":
                await CommandHandlers.DeletePingTrigger(command);
                break;
            
            case "info":
                await CommandHandlers.GetInfo(command);
                break;
        }
    }
    
}
