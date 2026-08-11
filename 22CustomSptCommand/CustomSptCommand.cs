using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando.SptCommands;
using SPTarkov.Server.Core.Helpers.Items;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services.Commerce;

namespace _22CustomSptCommand;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.customsptcommand";
    public string Name { get; init; } = "CustomCommandoCommandExample";
    public string Author { get; init; } = "SPTarkov";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    
    
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}

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
