using Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Client : BaseDomainEntity
{
    
    [Required, StringLength(100)]
    public required string CompanyName { get; set; } 
    [Required, StringLength(100)]
    public required string ContactName { get; set; } 
    [Required, StringLength(100)]
    public required string ContactEmail { get; set; } 
    [Required, StringLength(100)]
    public required string ContactPhone { get; set; } 
    [Required, StringLength(255)]
    public required string ContactAddress { get; set; }     
    [Required, StringLength(255)]
    public required string TradeLicenseNumber { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
 }