using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services.Commerce;
using SPTarkov.DI.Annotations;

namespace _23CustomAbstractChatBot.Commands;

// This class is only necessary if you want to use multiple prefixes for your commands
[Injectable]
public class MyCoolCustomPrefixCommands(MailSendService mailSendService) : ICustomChatCommand {

    public string GetCommandHelp(string command)
    {
        if (command == "test")
        {
            return "Usage: customPrefix test";
        }

        return null;
    }

    public ValueTask<string> Handle(string command, UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        if (command == "test")
        {
            mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"This is a test message shown as an example!");
            return ValueTask.FromResult(request.DialogId);
        }

        return new ValueTask<string>(string.Empty);
    }

    public string CommandPrefix
    {
        get => "customPrefix";
    }
    
    public List<string> Commands
    {
        get => ["test"];
    }
}
