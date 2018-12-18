namespace Digipolis.Correlation
{
    public class CorrelationOptions
    {
        /// <summary>
        /// Makes the correlationheader required for all routes that do not match property CorrelationHeaderNotRequiredRouteRegex.
        /// </summary>
        public bool CorrelationHeaderRequired { get; set; } = false;

        /// <summary>
        /// Routes that match this regex will never require a correlation header.
        /// By default the status routes /vx/status and /status will never require correlation.
        /// </summary>
        public string CorrelationHeaderNotRequiredRouteRegex { get; set; } = "^(/v./(?i)(status)/|/(?i)(status)/)";
    }
}
