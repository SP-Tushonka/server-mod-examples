using SPTarkov.Server.Core.Helpers.Dialogue;
using SPTarkov.Server.Core.Helpers.Dialogue.SPTFriend.Commands;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Commerce;
using SPTarkov.Server.Core.Services.Locales;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando;

namespace _23CustomAbstractChatBot;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.customabstractchatbot";
    public string Name { get; init; } = "CustomAbstractChatBotExample";
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

public class CustomAbstractChatBot : AbstractDialogChatBot
{
    public CustomAbstractChatBot(
        ISptLogger<AbstractDialogChatBot> logger,
        MailSendService mailSendService,
        ServerLocalisationService localisationService,
        IEnumerable<IChatCommand> chatCommands,
        IEnumerable<IChatMessageHandler> chatMessageHandlers
    ) : base(logger, mailSendService, localisationService, chatCommands)
    {
    }

    public override UserDialogInfo GetChatBot()
    {
        return new UserDialogInfo
        {
            Id = "674db14ed849a3727ef24da0", // REQUIRES a valid mongoid, use online generator to create one
            Aid = 1234566,
            Info = new UserDialogDetails
            {
                Level = 69,
                MemberCategory = MemberCategory.Developer,
                SelectedMemberCategory = MemberCategory.Developer,
                Nickname = "CoolAbstractChatBot",
                Side = "Bear"
            }
        };
    }

    protected override string GetUnrecognizedCommandMessage()
    {
        return "No clue what you are talking about bud!";
    }
}
