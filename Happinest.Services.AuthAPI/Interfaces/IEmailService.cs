using Happinest.Services.AuthAPI.CustomModels;

namespace Happinest.Services.AuthAPI.Interfaces
{
    /// <summary>
    /// Defines the contract for email-related operations including template management,
    /// email scheduling, and email sending functionality.
    /// Implementations provide methods for retrieving email templates, scheduling emails,
    /// and sending emails through SMTP configuration.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Retrieves an email template from database based on event type ID and template code.
        /// Replaces placeholders with actual event data (title, date, location, etc.).
        /// </summary>
        /// <param name="templateCode">The unique code identifying the email template.</param>
        /// <param name="eventId">The event ID for template customization. Use 0 for generic templates.</param>
        /// <returns>An <see cref="EmailTemplateBaseResponse"/> containing the populated email template.</returns>
        /// <remarks>
        /// - Supports both generic templates (eventId=0) and event-specific templates.
        /// - Generates registration URLs and formats dates/times appropriately.
        /// - Replaces template placeholders with actual event data.
        /// </remarks>
        //Task<EmailTemplateBaseResponse> GetEmailTemplate(string templateCode, long? eventId);

        /// <summary>
        /// Schedules an email for sending by creating EventEmail record with pending status.
        /// Gets email receivers from event guests with accepted invitations.
        /// </summary>
        /// <param name="request">The email scheduling request containing template and recipient details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// - Validates template exists and creates email with proper HTML body structure.
        /// - Creates EventEmail record with pending status for background processing.
        /// - Retrieves recipients from event guests with accepted invitation status.
        /// </remarks>
        //Task ScheduleEmail(ScheduleEmailRequest request);

        /// <summary>
        /// Sends an HTML email asynchronously using SMTP configuration.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="bodyHtml">The HTML content of the email body.</param>
        /// <param name="displayName">Optional display name for the sender.</param>
        /// <param name="fromEmail">Optional sender email address. If not provided, defaults to configured sender email.</param>
        /// <param name="replyToEmail">Optional reply-to email address.</param>
        /// <returns>A task representing the asynchronous email sending operation.</returns>
        /// <remarks>
        /// - Uses SMTP configuration from app settings.
        /// - Supports both HTML and plain text email formats.
        /// - Logs errors to CloudWatch if sending fails.
        /// </remarks>
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml, string? displayName = null, string? fromEmail = null, string replyToEmail = null);

        /// <summary>
        /// Retrieves all email templates from the database and returns them in a structured response.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains 
        /// an <see cref="EmailTemplateMasterBaseResponse"/> with the list of email templates and response metadata.
        /// </returns>
        //Task<EmailTemplateMasterBaseResponse> GetEmailTemplates();

        /// <summary>
        /// Adds a new email template or updates an existing one based on the presence of TemplateId.
        /// </summary>
        /// <param name="request">The email template data to be added or updated.</param>
        /// <returns>
        /// A <see cref="BaseResponse"/> indicating the success or failure of the operation,
        /// including validation messages and status codes.
        /// </returns>
        //Task<BaseResponse> SetEmailTemplateMaster(SetEmailTemplateMasterDto request);

        /// <summary>
        /// Deletes an email template based on the provided TemplateId.
        /// </summary>
        /// <param name="templateId">The unique identifier of the email template to delete.</param>
        /// <returns>A response indicating success or failure of the delete operation.</returns>
        //Task<BaseResponse> DeleteEmailTemplateMaster(int templateId);
    }
}
