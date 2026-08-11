using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Services.Commerce;

namespace _21CustomCommandoCommand;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.customcommandocommand";
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
