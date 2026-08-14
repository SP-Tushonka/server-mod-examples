using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;

namespace _12Bundle;

[Injectable(TypePriority = OnLoadOrder.PostLoad)]
public class BundleExample(ISptLogger<BundleExample> logger) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        logger.Success("Bundle example loaded!");
        return Task.CompletedTask;
    }
}
