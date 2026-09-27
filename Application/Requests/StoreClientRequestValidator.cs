using Application.Interfaces.Queries;
using Domain.Entities;
using FluentValidation;
using InvoiceHub.Application.Handlers.CommonHandlers;

namespace InvoiceHub.Application.Requests;

public class StoreClientRequestValidator : AbstractValidator<CreateRequest<Client>>
{
    public StoreClientRequestValidator(ICommonQueries<Client> clientRepo)
    {
        RuleFor(x => x.Entity.CompanyName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Entity.TradeLicenseNumber)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Entity.ContactEmail)
            .NotEmpty()
            .EmailAddress()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Entity.ContactAddress)
            .NotEmpty()
            .MinimumLength(256);

        RuleFor(x => x.Entity.ContactPhone)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-\(\)]+$")
            .WithMessage("Contact Phone must contain only digits and valid phone symbols.");
    }
}