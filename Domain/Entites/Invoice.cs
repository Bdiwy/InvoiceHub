using Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Invoice : BaseDomainEntity
{
    public required string InvoiceNumber { get; init; }
    public required string Title { get; set; } 
    public required string Description { get; set; } 
    public required decimal TotalAmount { get; set; } 
    public required decimal TaxAmount { get; set; }
    public required decimal DiscountAmount { get; set; }
    public required DateTime DueDate { get; set; }

    public required DateTime NotifiedAt { get; set; }

    public required DateTime PaidAt { get; set; }

    public required InvoiceStatus Status { get; set; }
    public required PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BANK_TRANSFER;
    public Guid ClientId { get; set; }
    public virtual required Client Client { get; set; }
}


public enum InvoiceStatus
{
    DRAFT,
    PENDING,
    PAID,
    OVERDUE,
    CANCELLED,
    REFUNDED,
    PARTIALLY_PAID,
    PARTIALLY_REFUNDED,
}

public enum PaymentMethod
{
    BANK_TRANSFER,
    CREDIT_CARD,
    DEBIT_CARD,
    PAYPAL,
    STRIPE,
    CASH,
    CHEQUE,
    OTHER,
}