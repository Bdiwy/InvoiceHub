using Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Client : BaseDomainEntity
{
    public required string CompanyName { get; set; } 
    public required string ContactName { get; set; } 
    public required string ContactEmail { get; set; } 
    public required string ContactPhone { get; set; } 
    public required string ContactAddress { get; set; }     
    public required string TradeLicenseNumber { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
 }