namespace Domain.Entities;

public partial class User
{
    public IEnumerable<string> Permissions => 
        Role?.RolePermissions?.Select(rp => rp.Permission.Name) ?? new List<string>();
    public virtual ICollection<AccessAndRefreshToken> Tokens { get; set; } = new HashSet<AccessAndRefreshToken>();
}
