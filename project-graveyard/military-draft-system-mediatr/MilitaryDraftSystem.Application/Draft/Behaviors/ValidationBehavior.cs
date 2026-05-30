using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MilitaryDraftSystem.Application.Draft.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            // check if validators exist for the request
            if(_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                // parallel validate the request and gather all validation errors
                var validationResults = _validators
                    .AsParallel()
                    .Select(v => v.Validate(context))
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();
                
                // if there are validation errors, throw an exception
                if (validationResults.Count != 0)
                {
                    throw new ValidationException(validationResults);
                }
            }

            return next();
        }
    }
}
