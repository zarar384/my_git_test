using MediatR;
using Microsoft.Extensions.Logging;

namespace MilitaryDraftSystem.Application.Draft.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            // Log the request details
            _logger.LogInformation("Handling {RequestName} with content: {@Request}", typeof(TRequest).Name, request);

            var response = next();

            // Log the response details
            _logger.LogInformation("Handled {RequestName} with response: {@Response}", typeof(TRequest).Name, response);

            return next();
        }
    }
}
