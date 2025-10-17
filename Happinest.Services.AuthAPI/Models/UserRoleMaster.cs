namespace Happinest.Services.AuthAPI.Models
{
    public partial class UserRoleMaster
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
