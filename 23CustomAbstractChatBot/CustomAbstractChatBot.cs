using SPTarkov.Server.Core.Helpers.Dialogue;
using SPTarkov.Server.Core.Helpers.Dialogue.SPTFriend.Commands;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Services.Commerce;
using SPTarkov.Server.Core.Services.Locales;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando;

namespace _23CustomAbstractChatBot;

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
