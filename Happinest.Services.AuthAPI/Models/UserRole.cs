namespace Happinest.Services.AuthAPI.Models
{
    public partial class UserRole
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public int RoleId { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual UserRoleMaster Role { get; set; } = null!;
        public virtual AppUser User { get; set; } = null!;

    }
}
    