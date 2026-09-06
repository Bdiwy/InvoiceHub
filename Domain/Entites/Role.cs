using System.ComponentModel.DataAnnotations;
using Domain.Entites;
using Domain.Interfaces;
namespace Domain.Entities
{

    public class Role : BaseDomainEntity
    {
        public required string Name { get; set; }
        public virtual ICollection<User?> Users { get; set; } = new HashSet<User?>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();


        public enum COFOUNDERS
        {
            OWNER,
            CEO,
            FOUNDER,
        }
    }
}
