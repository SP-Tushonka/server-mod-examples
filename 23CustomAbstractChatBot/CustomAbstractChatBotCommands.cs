namespace _23CustomAbstractChatBot;

using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Dialog;
using SPTarkov.Server.Core.Models.Eft.Profile;

[Injectable]
public class CustomAbstractChatBotCommands(IEnumerable<ICustomChatCommands> customChatCommands) : ICustomChatCommand
{
    private readonly IDictionary<string, ICustomChatCommands> _customChatCommands = customChatCommands.ToDictionary(c => c.Command);

    public string GetCommandHelp(string command)
    {
        return _customChatCommands.TryGetValue(command, out var value) ? value.CommandHelp : string.Empty;
    }

    public string CommandPrefix
    {
        get => "defaultPrefix";
    }

    public List<string> Commands
    {
        get => [.. _customChatCommands.Keys];
    }

    public async ValueTask<string> Handle(string command, UserDialogInfo commandHandler, MongoId sessionId, SendMessageRequest request)
    {
        return await _customChatCommands[command].PerformAction(commandHandler, sessionId, request);
    }
}