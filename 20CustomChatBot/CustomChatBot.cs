using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers.Dialogue;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Services.Commerce;

namespace _20CustomChatBot;

[Injectable]
public class CustomChatBot(
    MailSendService mailSendService) : IDialogueChatBot
{
    public UserDialogInfo GetChatBot()
    {
        return new UserDialogInfo
        {
            Id = "modderBuddy",
            Aid = 9999999,
            Info = new UserDialogDetails
            {
                Nickname = "Buddy",
                Side = "Usec",
                Level = 69,
                MemberCategory = MemberCategory.Sherpa,
                SelectedMemberCategory = MemberCategory.Sherpa
            }
        };
    }

    public ValueTask<string> HandleMessage(MongoId sessionId, SendMessageRequest request)
    {
        mailSendService.SendUserMessageToPlayer(
            sessionId,
            GetChatBot(),
            $"Im Buddy! I just reply back what you typed to me!\n{request.Text}");

        return ValueTask.FromResult(request.DialogId);
    }
}
