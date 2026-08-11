using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;

namespace _7UseMultipleClasses
{
    [Injectable(TypePriority = OnLoadOrder.PostLoad + 1)] // We flag our class as injectable and tell it where in the server load order to inject it
    public class SecondClass
    {
        /// <summary>
        /// This method will return text when called
        /// </summary>
        /// <returns>"test text"</returns>
        public string GetText()
        {
            return "test text";
        }
    }
}
