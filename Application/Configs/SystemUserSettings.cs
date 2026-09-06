
using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Configs;

public class SystemUserSettings: IRegisterAsSelf
{
    public const string SectionName = "SystemUserSettings";

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
