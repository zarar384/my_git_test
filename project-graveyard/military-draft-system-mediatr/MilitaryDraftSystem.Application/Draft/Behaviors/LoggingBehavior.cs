using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Draft.Behaviors
{
    /// <summary>
    /// Traces every MediatR request/response pair at Debug level. This is
    /// diagnostic noise by design (not Information), since significant
    /// business events are already logged by the handlers themselves.
    /// Exceptions are intentionally NOT caught/logged here - they propagate
    /// to the single global exception boundary (API middleware / hosted
    /// service catch blocks) so each failure is logged exactly once.
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling {RequestName}.", typeof(TRequest).Name);

            var response = await next();

            _logger.LogDebug("Handled {RequestName}.", typeof(TRequest).Name);

            return response;
        }
    }
}
