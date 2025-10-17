namespace Happinest.Services.AuthAPI.Models
{
    public partial class EventGuest
    {
        public long PersonalEventInviteId { get; set; }

        public long PersonalEventHeaderId { get; set; }

        public long InvitedBy { get; set; }

        public long? InviteTo { get; set; }

        public DateTime InvitedOn { get; set; }

        public int InviteVia { get; set; }

        public string? Email { get; set; }

        public string? Mobile { get; set; }

        public int InviteStatus { get; set; }

        public DateTime? AcceptedRejectedOn { get; set; }

        public bool IsCoHost { get; set; }

        public int AdditionalGuest { get; set; }

        public virtual AppUser? InviteToUser { get; set; }

        public virtual AppUser InvitedByUser { get; set; } = null!;
    }
}
