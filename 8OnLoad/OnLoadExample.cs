using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;

namespace _8OnLoad;

// Check `OnLoadOrder` for list of possible choices
[Injectable(TypePriority = OnLoadOrder.PostLoad)] // Can also give an int value for fine-grained control
public class OnLoadExample(
    ISptLogger<OnLoadExample> logger) : IOnLoad // Must implement the IOnLoad interface
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // Can do work here
        logger.Success($"Mod loaded after database!");

        return Task.CompletedTask;
    }
}
