using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers.Dialogue;
using SPTarkov.Server.Core.Helpers.Dialogue.Commando;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Services.Commerce;
using SPTarkov.Server.Core.Services.Locales;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;

namespace _23CustomAbstractChatBot;

[Injectable]
public class CustomAbstractChatBot(
    ISptLogger<AbstractDialogChatBot> logger,
    MailSendService mailSendService,
    ServerLocalisationService localisationService,
    IEnumerable<ICustomChatCommand> chatCommands
    ) : AbstractDialogChatBot (logger, mailSendService, localisationService, chatCommands)
{

    public override UserDialogInfo GetChatBot()
    {
        return new UserDialogInfo
        {
            Id = "6a7f47e5b8787dcdc0eb46df", // REQUIRES a valid mongoid, use online generator to create one
            Aid = 819476, // Set a unique AID
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

// Register our chatbot so it displays on our friends list
[Injectable(TypePriority = OnLoadOrder.PostLoad)]
public class RegisterChatbot(
    CoreConfig coreConfig,
    CustomAbstractChatBot customAbstractChatBot): IOnLoad 
{
    public Task OnLoadAsync(CancellationToken token)
    {
        var myCustomAbstractChatBot = customAbstractChatBot.GetChatBot();
        
        coreConfig.Features.ChatbotFeatures.Ids[myCustomAbstractChatBot.Id] = myCustomAbstractChatBot.Id;
        coreConfig.Features.ChatbotFeatures.EnabledBots[myCustomAbstractChatBot.Id] = true;
        
        return Task.CompletedTask;
    }
}