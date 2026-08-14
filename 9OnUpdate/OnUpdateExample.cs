using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;

namespace _9OnUpdate;

// Check `OnUpdateOrder` for list of possible choices
[Injectable(TypePriority = OnUpdateOrder.InsuranceCallbacks)] // Can also give it an int value for more fine-grained control
public class OnUpdateExample(
    ISptLogger<OnUpdateExample> logger) : IOnUpdate // Must implement the IOnUpdate interface
{
    public Task<bool> OnUpdateAsync(long secondsSinceLastRun, CancellationToken cancellationToken)
    {
        // Can do work here
        logger.Success($"Mod running update after insurance callbacks have run!");

        return Task.FromResult(true); // Return true for a success, false for failure
    }
}
