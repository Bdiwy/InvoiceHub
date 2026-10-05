using Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Invoice : BaseDomainEntity
{
    public Invoice(
        string invoiceNumber,
        string title,
        string description,
        decimal totalAmount,
        decimal taxAmount,
        decimal discountAmount,
        DateTime dueDate
    )
    {
        InvoiceNumber = invoiceNumber;
        Title = title;
        Description = description;
        TotalAmount = totalAmount;
        TaxAmount = taxAmount;
        DiscountAmount = discountAmount;
        DueDate = dueDate;
    }
    [IncludeInResponse] public string InvoiceNumber { get; init; }
    [IncludeInResponse] public string Title { get; set; } 
    [IncludeInResponse] public string Description { get; set; } 
    [IncludeInResponse] public decimal TotalAmount { get; set; } 
    [IncludeInResponse] public decimal TaxAmount { get; set; }
    [IncludeInResponse] public decimal DiscountAmount { get; set; }
    [IncludeInResponse] public DateTime DueDate { get; set; }
    [IncludeInResponse] public DateTime NotifiedAt { get; set; }
    [IncludeInResponse] public DateTime PaidAt { get; set; }
    [IncludeInResponse] public InvoiceStatus Status { get; set; }
    [IncludeInResponse] public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BANK_TRANSFER;
    [IncludeInResponse] public Guid ClientId { get; set; }
    public Client? Client { get; set; }
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