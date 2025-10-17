namespace Happinest.Models;

public partial class EmailTemplateMaster
{
    public int TemplateId { get; set; }

    public string? TemplateCode { get; set; }

    public string? EmailSubject { get; set; }

    public string? EmailBody { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public int? ModifyBy { get; set; }

    public int? EventTypeMasterId { get; set; }

    public string? EmailHeader { get; set; }

    public string? EmailFooter { get; set; }

    public string Language { get; set; } = null!;

    public bool IsActive { get; set; }
    
   // public virtual EventTypeMaster EventType { get; set; }
    //public virtual ICollection<EventEmail> EventEmails { get; set; } = new List<EventEmail>();
}
