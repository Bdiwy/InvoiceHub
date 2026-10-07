using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Domain.Interfaces;

namespace Domain.Entities
{
    public partial class User : ITenantEntity
    {
        public User(Guid newTenantId, Guid roleId, string username, string email, bool isOwner, string phoneNumber)
        {
            RoleId = roleId;
            TenantId = newTenantId;
            Username = username;
            Email = email;
            IsOwner = isOwner;
            PhoneNumber = phoneNumber;
        }
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Indicates if the user is the owner of the tenant/Company. This property is used to determine if the user has special privileges or permissions within the system. By default, 
        /// this property is set to false, meaning the user is not the owner unless explicitly specified otherwise.
        /// </summary>
        public bool IsOwner { get; set; } = false;
        public Guid? TeamId { get; set; }
        public virtual Team? Team { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? RoleId { get; set; }
        public Role? Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}