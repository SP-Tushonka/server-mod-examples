using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;

namespace _12Bundle
{
    public record ModMetadata : IModMetadata
    {
        public string Name { get; init; } = "BundleExample";
        public string Author { get; init; } = "SPTarkov";
        public List<string>? Contributors { get; init; }
        public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
        public SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
        
        
        public List<string>? Incompatibilities { get; init; }
        public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public string? Url { get; init; }
        public string License { get; init; } = "MIT";
        public string ModGuid { get; init; } = "com.sp-tarkov.examples.bundleexample";
        public bool HasPrepatcher { get; init; } = false;
    }

    [Injectable(TypePriority = OnLoadOrder.PostLoad)]
    public class BundleExample(ISptLogger<BundleExample> logger) : IOnLoad
    {
        public Task OnLoadAsync(CancellationToken cancellationToken)
        {
            logger.Success("Bundle example loaded!");
            return Task.CompletedTask;
        }
    }
}
