using System.ComponentModel.DataAnnotations;
using Domain.Entites;
using Domain.Interfaces;
namespace Domain.Entities
{

    public class Team : BaseDomainEntity
    {

        public required string Name { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
