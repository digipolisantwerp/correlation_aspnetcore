namespace Digipolis.Correlation.Helpers
{
    public interface IScopedCorrelationContext
    {
        CorrelationContext Context { get; set; }
    }

    public class ScopedCorrelationContext : IScopedCorrelationContext
    {
        public CorrelationContext Context { get; set; }
    }
}
