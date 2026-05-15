using Application.Interfaces.Queries;
using Domain.Entities;
using FluentValidation;
using InvoiceHub.Application.Handlers.ClientHandlers;

namespace InvoiceHub.Application.Requests;

public class StoreClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    public StoreClientRequestValidator(ICommonQueries<Client> clientRepo)
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.TradeLicenseNumber)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .EmailAddress()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.ContactAddress)
            .NotEmpty()
            .MinimumLength(256);

        RuleFor(x => x.ContactPhone)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-\(\)]+$")
            .WithMessage("Contact Phone must contain only digits and valid phone symbols.");
    }
}