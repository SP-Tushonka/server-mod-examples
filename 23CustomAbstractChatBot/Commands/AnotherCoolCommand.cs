using SPTarkov.Server.Core.Helpers.Dialogue.Commando;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services.Commerce;

namespace _23CustomAbstractChatBot.Commands;

public class AnotherCoolCommand : IChatCommand
{
    private readonly MailSendService _mailSendService;

    public AnotherCoolCommand(
        MailSendService mailSendService
    )
    {
        _mailSendService = mailSendService;
    }

    public string? GetCommandHelp(string command)
    {
        if (command == "test")
        {
            return "Usage: anotherExample test";
        }

        return null;
    }

    public string CommandPrefix => "anotherExample";
    public List<string> Commands => ["test"];

    public ValueTask<string> Handle(string command, UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        if (command == "test")
        {
            _mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"This is another test message shown as a different example!");
            return ValueTask.FromResult(request.DialogId);
        }

        return new ValueTask<string>(string.Empty);
    }
}
