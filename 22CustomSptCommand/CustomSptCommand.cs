using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando.SptCommands;
using SPTarkov.Server.Core.Helpers.Items;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Services.Commerce;

namespace _22CustomSptCommand;

[Injectable]
public class CustomSptCommand(
    MailSendService mailSendService,
    ItemHelper itemHelper) : ISptCommand
{
    public ValueTask<string> PerformAction(UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        var splitCommand  = request.Text.Split(" ");
        mailSendService.SendUserMessageToPlayer(sessionId, commandHandler, $"That templateId belongs to item {itemHelper.GetItem(splitCommand[2]).Value?.Properties?.Name ?? ""}");
        
        return ValueTask.FromResult(request.DialogId);
    }

    public string Command => "getName";

    public string CommandHelp => "Usage: spt getName tplId";
}
