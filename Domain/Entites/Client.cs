using Domain.Entites;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Client : BaseDomainEntity
{
    public Client(
        string companyName,
        string contactName,
        string contactEmail,
        string contactPhone,
        string contactAddress,
        string tradeLicenseNumber)
    {
        CompanyName = companyName;
        ContactName = contactName;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        ContactAddress = contactAddress;
        TradeLicenseNumber = tradeLicenseNumber;
    }
    [IncludeInResponse]
    public string CompanyName { get; set; } 
    [IncludeInResponse]
    public string ContactName { get; set; } 
    [IncludeInResponse]
    public string ContactEmail { get; set; } 
    [IncludeInResponse]
    public string ContactPhone { get; set; } 
    [IncludeInResponse]
    public string ContactAddress { get; set; }     
    [IncludeInResponse]
    public string TradeLicenseNumber { get; set; }
    public IEnumerable<Invoice>? Invoices { get; set; }
 }