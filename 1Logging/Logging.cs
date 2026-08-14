using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;

namespace _1Logging;

// We want to load after Preload is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.Preload + 1)]
public class Logging(
    ISptLogger<Logging> logger) // We inject a logger for use inside our class, it must have the class inside the diamond <> brackets
    : IOnLoad // Implement the IOnLoad interface so that this mod can do something on server load
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // We can access the logger and call its methods to log to the server window and the server log file
        logger.Success("This is a success message");
        logger.Warning("This is a warning message");
        logger.Error("This is an error message");
        logger.Info("This is an info message");
        logger.Critical("This is a critical message");

        // Logging with colors requires you to 'pass' the text color and background color
        logger.LogWithColor("This is a message with custom colors", Spectre.Console.Color.Red, Spectre.Console.Color.Black);
        logger.Debug("This is a debug message that gets written to the log file, not the console");
        
        // Inform the server our mod has finished doing work
        return Task.CompletedTask;
    }
}
