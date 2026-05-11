using Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Invoice : BaseDomainEntity
{
    [Required, StringLength(100)]
    public required string Title { get; set; } 
    [Required]
    public required string Description { get; set; } 
    [Required]
    public required decimal TotalAmount { get; set; } 
    [Required]
    public required decimal TaxAmount { get; set; }
    [Required]
    public required decimal DiscountAmount { get; set; }
    [Required]
    public required DateTime DueDate { get; set; }

    [Required]
    public required DateTime NotifiedAt { get; set; }

    [Required]
    public required DateTime PaidAt { get; set; }

    [Required]
    public required InvoiceStatus Status { get; set; }
    [Required]
    public required PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BANK_TRANSFER;
    [Required]
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