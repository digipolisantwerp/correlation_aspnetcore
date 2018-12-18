namespace Digipolis.Correlation
{
    public interface ICorrelationContextFormatter
    {
        CorrelationContext ValidateAndSetPropertiesFromDgpHeader(string header);
    }
}