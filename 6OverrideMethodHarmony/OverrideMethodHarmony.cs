using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Services.Server;
using System.Reflection;

namespace _6OverrideMethodHarmony;

[Injectable(TypePriority = OnLoadOrder.Preload)]
public class StartAsyncHarmonyPatchExample(
    IEnumerable<IRuntimePatch> patches,
    ISptLogger<StartAsyncHarmonyPatchExample> logger) : IOnLoad
{
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        // You will need to enable your patch in an OnLoad, preferably during Preload
        foreach (var patch in patches)
        {
            patch.Enable();
        }

        logger.Success($"PerformPostDbLoadActions harmony patch has successfully loaded!");

        return Task.CompletedTask;
    }
}

[Injectable]
public class StartAsyncPatch : AbstractPatch
{
    private static ISptLogger<StartAsyncPatch> _logger = default!;

    public StartAsyncPatch(ISptLogger<StartAsyncPatch> logger)
    {
        _logger = logger;
    }

    protected override MethodBase GetTargetMethod()
    {
        return typeof(PostDbLoadService).GetMethod(nameof(PostDbLoadService.PerformPostDbLoadActions)) ?? throw new InvalidOperationException("Could not find target method!");
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        // We add a log message to the StartAsync method
        _logger.Success("This is a StartAsync harmony patch mod override!");

        // You can perform any code here before the method actually runs

        // This runs the original method, if it is set to false, it will skip the original method
        return true;
    }

    [PatchPostfix]
    public static async Task Postfix(Task __result)
    {
        // Optionally here you could modify the result after it has run, or run code afterwards
        _logger.Success("StartAsync harmony patch OnLoad has ran!");

        // Have to await a result here because of async, this will not be necessary on a non-async method
        await __result;
    }
}
