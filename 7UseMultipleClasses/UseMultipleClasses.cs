using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;

namespace _7UseMultipleClasses;

/// <summary>
/// Having multiple classes can make keeping your code maintainable easier, you can split related code into their own class and inject them
/// </summary>

// We want to load after PostDBModLoader is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class UseMultipleClasses(
    ISptLogger<UseMultipleClasses> logger,
    SecondClass secondClass // We inject our second class just like other classes
    ) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // We call the "GetText" method that exists in the other class
        var text = secondClass.GetText();

        // Log the result to the server console
        logger.Info($"The SecondClass returned the text: {text}");
        
        // Tell server we've finished
        return Task.CompletedTask;
    }
}
