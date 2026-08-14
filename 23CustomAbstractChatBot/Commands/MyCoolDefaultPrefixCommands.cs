using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services.Commerce;
using SPTarkov.DI.Annotations;

namespace _23CustomAbstractChatBot.Commands;

// Duplicate this class for each command you want that will use the defaultPrefix from CustomAbstractChatBotCommands
[Injectable]
public class MyCoolDefaultPrefixCommands(MailSendService mailSendService) : ICustomChatCommands 
{
    public string Command
    {
        get => "test";
    }
    
    public string CommandHelp
    {
        get => "Usage: defaultPrefix test";
    }

    public ValueTask<string> PerformAction(UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        mailSendService.SendUserMessageToPlayer(sessionId, commandHandler,"This is a test message shown as an example!");
        return new ValueTask<string>(request.DialogId);
    }
}
