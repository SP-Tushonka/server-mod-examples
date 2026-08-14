using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace _21CustomCommandoCommand;

[Injectable]
public class CustomCommandoCommand(
    MailSendService mailSendService,
    GlobalTable globalTable) : IChatCommand
{
    public string GetCommandHelp(string command)
    {
        if (command == "talk")
        {
            return "Usage: test talk";
        }

        return null;
    }

    public ValueTask<string> Handle(string command, UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        if (command == "talk")
        {
            mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"IM TALKING! OKAY?!\nHere's the walk speed X config from the DB: {globalTable.Configuration.WalkSpeed.X}");
            return new ValueTask<string>(request.DialogId);
        }

        return new ValueTask<string>(string.Empty);
    }

    public string CommandPrefix { get; }
    public List<string> Commands => ["talk"];
}
