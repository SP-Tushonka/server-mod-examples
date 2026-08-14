namespace _23CustomAbstractChatBot;

using SPTarkov.Server.Core.Helpers.Dialogue.Commando;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;

// You should use your own name for this so you don't pull in commands from other authors who use this example
public interface ICustomChatCommands 
{
    public string Command { get; }
    public string CommandHelp { get; }
    public ValueTask<string> PerformAction(UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request);
}

// For creating custom prefixes - you should also use your own name for this
public interface ICustomChatCommand : IChatCommand { }