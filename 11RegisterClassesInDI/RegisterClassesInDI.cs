using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Common.Models.Logging;

namespace _11RegisterClassesInDI;

// We want to load after PostLoad is complete, so we set our type priority to that, plus 1.
[Injectable(TypePriority = OnLoadOrder.PostLoad + 1)]
public class RegisterClassesInDi(
    SingletonClassExample singletonClassExample,
    ScopedClassExample scopedClassExample)
    : IOnLoad
{
    // We inject 2 classes (singleton and scoped) we've made below

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        singletonClassExample.IncrementCounterAndLog();
        singletonClassExample.IncrementCounterAndLog();
        singletonClassExample.IncrementCounterAndLog();

        scopedClassExample.IncrementCounterAndLog();
        scopedClassExample.IncrementCounterAndLog();
        scopedClassExample.IncrementCounterAndLog();
        
        return Task.CompletedTask;
    }
}

// This class is registered as a singleton. This means ONE and only ONE instance
// of this class will ever exist.
[Injectable(InjectionType.Singleton)]
public class SingletonClassExample(ISptLogger<SingletonClassExample> logger)
{
    private int _counter = 0;

    public void IncrementCounterAndLog()
    {
        _counter++;
        logger.Success($"{_counter}");
    }
}

// This class is being registered as default or scoped. This means that
// every time a class requests an instance of this type a new one will be created
[Injectable(InjectionType.Scoped)] // [Injectable] is the same as doing this
public class ScopedClassExample(
    ISptLogger<ScopedClassExample> logger)
{
    private int _counter = 0;

    public void IncrementCounterAndLog()
    {
        _counter++;
        logger.Success($"{_counter}");
    }
}
