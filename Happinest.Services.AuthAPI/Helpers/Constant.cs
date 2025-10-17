using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Happinest.Services.AuthAPI.Helpers
{
    public class Constant
    {

        public enum StatusCode
        {
            Success = 1,
            Deleted = 2,
            Updated = 3,
            NotFound = 4,
            RecordAlreadyExist = 5,
            EmailDoesNotExist = 6,
            PasswordNotMatch = 7,
            UserNameExist = 8,
            UserNotVerify = 9,
            BadRequest = 10,
            Error = 11,
            Unauthorized = 12,
            Failed = 13,
            Invalid = 14,
            Forbidden = 15,
            Conflict = 16
        }

        public enum NotificationType
        {
            TRAVELORYTNCMSG = 1,
            FRIENDREQUEST = 2,
            COAUTHORREQUEST = 3,
            TRIPLIKE = 4,
            TRIPCOMMENT = 5,
            TRIPSHARE = 6,
            NEWTRIP = 7,
            FLAGTRIP = 8,
            TRAVELORYPPMSG = 9,
            COAUTHORACCEPT = 11,
            COAUTHORREJECT = 12,
            COAUTHORLEFT = 13,
            COAUTHORREMOVED = 14,
            FRIENDREQUESTACCEPT = 15,
            FRIENDREQUESTREJECT = 16,
            EVENTINVITE = 25,
            EVENTINVITEACCEPT = 26,
            EVENTINVITEREJECT = 27,
            EVENTLIKE = 28,
            EVENTCOMMENT = 29,
            MAKECOHOST = 30

        }

        public enum InviteVia
        {
            Email = 1,
            Mobile = 2,
            UserId = 3,
            RegistrationPortal = 4
        }

        /// <summary>
        /// Defines Visibility levels for a event.
        /// </summary>
        public enum EventVisibility
        {
            /// <summary>
            /// Visible to everyone.
            /// </summary>
            Public = 1,

            /// <summary>
            /// Visible only to the event creator and co-hosts.
            /// </summary>
            Private = 2,

            /// <summary>
            /// Visible to the event creator, co-hosts, and invited guests.
            /// </summary>
            Guests = 3,

            /// <summary>
            /// Visible to the event creator, co-hosts, invited guests, and followers of the event creator.
            /// </summary>
            Followers = 4
        }

        /// <summary>
        /// Represents the status of an event invitation.
        /// </summary>
        public enum EventInviteStatus
        {
            /// <summary>
            /// The invitation is pending and awaiting response.
            /// </summary>
            Pending = 1,
            /// <summary>
            /// The invitation has been accepted.
            /// </summary>
            Accepted = 2,
            /// <summary>
            /// The invitation has been rejected.
            /// </summary>
            Rejected = 3,
            /// <summary>
            /// The invitation has been deleted.
            /// </summary>
            Deleted = 4,
            /// <summary>
            /// The user has registered for the event.
            /// </summary>
            Registered = 5,
            /// <summary>
            /// The user has attended the event.
            /// </summary>
            Attended = 6
        }


        /// <summary>
        /// Represents the status of a video processing request.
        /// </summary>
        public enum VideoStatus
        {
            /// <summary>
            /// The video is currently being processed.
            /// </summary>
            InProgress = 1,
            /// <summary>
            /// The video processing is completed.
            /// </summary>
            Completed = 2,
            /// <summary>
            /// The video processing has not been requested.
            /// </summary>
            NotRequested = 3
        }

        /// <summary>
        /// Represents different types of guests.
        /// </summary>
        public enum GuestType
        {
            /// <summary>
            /// The primary host.
            /// </summary>
            Host = 1,

            /// <summary>
            /// A co-host assisting the main host.
            /// </summary>
            CoHost = 2,

            /// <summary>
            /// A regular guest.
            /// </summary>
            Guest = 3
        }

        /// <summary>
        /// Represents the status of a follow request.
        /// </summary>
        public enum FollowRequestStatus
        {
            /// <summary>
            /// The follow request status is unknown.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// The follow request is pending approval.
            /// </summary>
            Pending = 1,

            /// <summary>
            /// The follow request has been accepted.
            /// </summary>
            Accepted = 2,

            /// <summary>
            /// The follow request has been rejected.
            /// </summary>
            Rejected = 3,

            /// <summary>
            /// The follow request has been cancelled by the requester.
            /// </summary>
            Canceled = 4
        }


        /// <summary>
        /// Enum representing various notification events that a user can receive.
        /// </summary>
        public enum NotificationEvent
        {
            /// <summary>
            /// Notification when someone likes a user's story.
            /// </summary>
            LikeOnUsersStory = 1,

            /// <summary>
            /// Notification when someone comments on a user's story.
            /// </summary>
            CommentOnUsersStory = 2,

            /// <summary>
            /// Notification when someone comments on a user's post.
            /// </summary>
            CommentOnUsersPost = 3,

            /// <summary>
            /// Notification for a new event invite.
            /// </summary>
            NewEventInvite = 4,

            /// <summary>
            /// Notification when a guest accepts an event invite.
            /// </summary>
            GuestAcceptedInvite = 5,

            /// <summary>
            /// Notification when a guest declines an event invite.
            /// </summary>
            GuestDeclinedInvite = 6,

            /// <summary>
            /// Notification reminding about an event, two days or one day before the event.
            /// </summary>
            EventReminderTwoDaysBefore = 7,

            /// <summary>
            /// Notification reminding about an event, one day before the event.
            /// </summary>
            EventReminderOneDayBefore = 8,

            /// <summary>
            /// Notification when there is a new follow request.
            /// </summary>
            NewFollowRequest = 9,

            /// <summary>
            /// Notification when a user gains new followers.
            /// </summary>
            NewFollower = 10,


            /// <summary>
            /// 
            /// </summary>
            FollowRequestAccepted = 11,

            /// <summary>
            /// Notification about popular events.
            /// </summary>
            PopularEvents = 12,

            /// <summary>
            /// To notify the post creator when their post is hidden (e.g., due to a policy violation)
            /// </summary>
            HideMoment = 13,

            /// <summary>
            /// Notification when there is a new follow request rejected.
            /// </summary>
            FollowRequestRejected = 20,

            /// <summary>
            /// Notification when the HappiVid video is successfully generated and ready to view/share.
            /// </summary>
            HappiVidReady = 14,

            /// <summary>
            /// Notification when the HappiVid video generation has failed.
            /// </summary>
            HappiVidFailed = 15,

            /// <summary>
            /// Notification when the Invitation video is successfully generated and ready to view/share.
            /// </summary>
            InvitationReady = 16,

            /// <summary>
            /// Notification when the Invitation video generation has failed.
            /// </summary>
            InvitationFailed = 17,

            /// <summary>
            /// Indicates that the event has been successfully created and is ready to view and share.
            /// Used to notify the user that their event setup is complete.
            /// </summary>
            EventCreated = 18,
            /// <summary>
            /// Indicates that the event creation has failed due to an unexpected error.
            /// Used to notify the user that their event setup could not be completed.
            /// </summary>
            EventFailed = 19
        }

        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// Defines the default sorting options for events.
        /// </summary>
        public enum DefaultEventsFilterBy
        {
            /// <summary>
            /// Sort events by the most recently created or updated.
            /// </summary>
            Recent,

            /// <summary>
            /// Sort events that are currently trending.
            /// </summary>
            Trending,

            /// <summary>
            /// Sort events by popularity (e.g., most viewed, liked, or attended).
            /// </summary>
            Popular,

            /// <summary>
            /// Sort events recommended for the user, based on preferences or activity.
            /// </summary>
            Recommended
        }

        /// <summary>
        /// Represents the status of a processed email.
        /// </summary>
        public enum ProcessedEmailStatus
        {
            /// <summary>
            /// The email has been sent successfully.
            /// </summary>
            Sent,
            /// <summary>
            /// The email is pending and has not been sent yet.
            /// </summary>
            Pending,
            /// <summary>
            /// The email failed to send.
            /// </summary>
            Failed
        }

        /// <summary>
        /// Represents the type of an event.
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// A wedding event.
            /// </summary>
            Wedding,

            /// <summary>
            /// A baby shower celebration.
            /// </summary>
            Babyshower,

            /// <summary>
            /// A birthday party or celebration.
            /// </summary>
            Birthday,

            /// <summary>
            /// An anniversary celebration.
            /// </summary>
            Anniversary,

            /// <summary>
            /// A sports-related event or gathering.
            /// </summary>
            Sports,

            /// <summary>
            /// A music or entertainment concert.
            /// </summary>
            Concert,

            /// <summary>
            /// A startup-related event.
            /// </summary>
            Startup,

            /// <summary>
            /// A technology-related event.
            /// </summary>
            Tech,

            /// <summary>
            /// A movie or show premiere event.
            /// </summary>
            Premier,

            /// <summary>
            /// A personal event or gathering.
            /// </summary>
            Personal,

            /// <summary>
            /// A house warming celebration.
            /// </summary>
            HouseWarming,

            /// <summary>
            /// A travel or trip-related event.
            /// </summary>
            Travel,

            /// <summary>
            /// A test or placeholder event.
            /// </summary>
            TestEvent,

            /// <summary>
            /// A conference or seminar.
            /// </summary>
            Conference
        }

        /// <summary>
        /// Specifies the source of an event, such as whether it was created as a moment or as a full event.
        /// </summary>
        public enum EventSource
        {
            /// <summary>
            /// Indicates the event was created as a moment.
            /// </summary>
            CreateMoment,

            /// <summary>
            /// Indicates the event was created as a full event.
            /// </summary>
            CreateEvent,
        }

        public enum RequestType
        {
            Wedding,
            PreWedding,
            Invitation,
            PreBirthday,
            Birthday,
            Travel,
            PostTravel,
            General
        }

        public enum AuthPolicies
        {
            SuperAdminOrAdminPolicy,
            SuperAdminPolicy,
            AdminPolicy,
            UserPolicy,
            GuestPolicy
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum UserRoles
        {
            [EnumMember(Value = "System")]
            System,

            [EnumMember(Value = "SuperAdmin")]
            SuperAdmin,

            [EnumMember(Value = "Admin")]
            Admin,

            [EnumMember(Value = "User")]
            User,

            [EnumMember(Value = "Guest")]
            Guest
        }

        public enum VideoRenderStatus
        {
            Processing,
            Timeout,
            Success,
            Error,
            Submitted,
            Generating,
            Failed,
            Analyzing,
            ReSubmitted,
            Selecting
        }

        public enum MediaAnalysisStatus
        {
            Processing,
            Timeout,
            Success,
            Error,
            Submitted,
            Generating,
            Failed,
        }

        public enum VideoRequestType
        {
            Recap,
            Invitation
        }

        /// <summary>
        /// Enum representing the source of user authentication.
        /// Used to determine which login flow to execute based on the user's chosen authentication method.
        /// </summary>
        public enum AuthenticationSource
        {
            /// <summary>
            /// Login using the application's native username and password form.
            /// </summary>
            InApp = 0,

            /// <summary>
            /// Login using a Google account via OAuth and ID token validation.
            /// </summary>
            Google = 1,

            /// <summary>
            /// Login using an Instagram account via external authentication (OAuth).
            /// </summary>
            Instagram = 2,

            /// <summary>
            /// Login using a Meta account via access token validation through Meta's Graph API.
            /// </summary>
            Meta = 3
        }

        public enum VideoIcons
        {
            Platform,
            RequestType,
            Styles
        }

        /// <summary>
        /// Enum to define sorting options for the HappiFeed video list.
        /// </summary>
        public enum HappiVidsSortBy
        {
            /// <summary>
            /// Sort by most liked videos.
            /// </summary>
            Top = 0,

            /// <summary>
            /// Sort by most recently shared videos.
            /// </summary>
            Recent = 1
        }


        /// <summary>
        /// Specifies the type of weightage configuration being applied.
        /// Used to determine scoring or ranking logic for various entities.
        /// </summary>
        public enum WeightageType
        {
            /// <summary>
            /// Weightage applied to determine trending events based on specific criteria.
            /// </summary>
            TrendingEvents,

            /// <summary>
            /// Weightage applied to determine popular events based on factors like engagement.
            /// </summary>
            PopularEvents,

            /// <summary>
            /// Weightage applied during photo selection logic for analysis or video generation.
            /// </summary>
            PhotoSelection,

            /// <summary>
            /// Weightage applied to rank or highlight popular users in the system.
            /// </summary>
            PopularUsers
        }


        /// <summary>
        /// Represents keys for dynamic placeholders used in invitation templates.
        /// </summary>
        public enum InvitationPlaceholderKey
        {
            /// <summary>
            /// Placeholder for the first partner's full name (e.g., {{partner1}}).
            /// </summary>
            Partner1,

            /// <summary>
            /// Placeholder for the second partner's full name (e.g., {{partner2}}).
            /// </summary>
            Partner2,

            /// <summary>
            /// Placeholder for the combined full names of both partners (e.g., {{partner1}} & {{partner2}}).
            /// </summary>
            Partner1AndPartner2,

            Partner1AndPartner2WithLineBreaks,

            Partner1Initial,
            Partner2Initial,
            /// <summary>
            /// Placeholder for the combined initials of both partners (e.g., {{partner1.initial}} & {{partner2.initial}}).
            /// </summary>

            Partner1InitialAndPartner2Initial,

            /// <summary>
            /// Placeholder for the partner's age (e.g., {{age}}).
            /// </summary>
            Age,

            /// <summary>
            /// Placeholder for the partner's gender (e.g., {{gender}}).
            /// </summary>

            Gender,

            /// <summary>
            /// Placeholder for the expected baby's gender (e.g., {{babyGender}}).
            /// </summary>
            BabyGender,

            /// <summary>
            /// Placeholder for the name of the host (e.g., {{host.name}}).
            /// </summary>
            HostName,

            /// <summary>
            /// Placeholder for the event title (e.g., {{event.title}}).
            /// </summary>
            EventTitle,

            /// <summary>
            /// Placeholder for the event location (e.g., {{event.location}}).
            /// </summary>
            EventLocation,

            /// <summary>
            /// Placeholder for the event date (e.g., {{event.date}}).
            /// </summary>
            EventDate,
            /// <summary>
            /// Placeholder for the event start time (e.g., {{event.starttime}}).
            /// </summary>
            EventStartTime,
            /// <summary>
            /// Placeholder for the activity name associated (e.g., {{activity.name}}).
            /// </summary>
            ActivityName,

            /// <summary>
            /// Placeholder for the activity start time associated (e.g., {{activity.starttime}}).
            /// </summary>
            ActivityStartTime,

            /// <summary>
            /// Placeholder for the event title without the last word 
            /// (e.g., "Rio Carnival" from {{event.title_noLastWord}}).
            /// </summary>
            EventTitleNoLastWord,

            /// <summary>
            /// Placeholder for the last word of the event title 
            /// (e.g., "Adventure" from {{event.title_LastWord}}).
            /// </summary>
            EventTitleLastWord,
        }

        /// <summary>
        /// Defines the types of email templates used in the Happinest system.
        /// </summary>
        public enum EmailTemplateType
        {
            /// <summary>
            /// Template for user feedback notification.
            /// </summary>
            USERFEEDBACK,

            /// <summary>
            /// Template used when a trip is reported by a user.
            /// </summary>
            REPORT_TRIP,

            /// <summary>
            /// Template containing admin response to user feedback.
            /// </summary>
            ADMINRESPONSEFORFEEDBACK,

            /// <summary>
            /// Template used to notify about a trip report response.
            /// </summary>
            REPORT_TRIP_RESPONSE,

            /// <summary>
            /// Reminder email template for upcoming events.
            /// </summary>
            EventReminder,

            /// <summary>
            /// Thank you email template after an event concludes.
            /// </summary>
            EventThankYou,

            /// <summary>
            /// Placeholder for existing or generic templates.
            /// </summary>
            ExistingTemplate,

            /// <summary>
            /// Template used when a user's password is reset.
            /// </summary>
            RESETPASSWORD,
            /// <summary>
            /// Template used to send messages submitted via the Contact Us form.
            /// </summary>
            CONTACTUS,
            /// <summary>
            /// Email sent when a user successfully registers for an event.
            /// </summary>
            REGISTRATIONSUCCESSFUL,

            /// <summary>
            /// Email sent to guests with the event invitation and registration link.
            /// </summary>
            GUESTINVITE
        }

        /// <summary>
        /// Represents known error codes that can occur during video rendering or processing.
        /// These codes are used to map exceptions to database-stored error entries.
        /// </summary>
        public enum VideoErrorCode
        {
            /// <summary>
            /// A generic error occurred during the video generation process.
            /// </summary>
            VideoGenerationFailed,

            /// <summary>
            /// The video rendering process timed out.
            /// </summary>
            Timeout,

            /// <summary>
            /// An error occurred during image analysis using OpenAI services.
            /// </summary>
            AI_Failure,

            /// <summary>
            /// The video rendering server was unreachable or failed to respond.
            /// </summary>
            ServerDown,

            /// <summary>
            /// A concurrency conflict occurred, such as a row being updated or deleted by another process.
            /// </summary>
            ConcurrencyConflict,

            /// <summary>
            /// A database-related error occurred, such as a SQL exception or foreign key constraint violation.
            /// </summary>
            DatabaseError,


            /// <summary>
            /// A transient or unknown failure occurred. This error might be resolved by retrying the operation.
            /// </summary>
            RetryableFailure,

            /// <summary>
            /// The request exceeded a rate limit or quota. Retrying after a delay may succeed.
            /// </summary>
            RateLimitExceeded,

            /// <summary>
            /// The input provided was invalid or incorrectly formatted, resulting in a failure to process the request.
            /// </summary>
            InvalidInput,
        }

        /// <summary>
        /// Specifies the timing context for a video request,
        /// indicating whether the request is for a past event or a future event.
        /// </summary>
        public enum VideoRequestTypeRequestTiming
        {
            /// <summary>
            /// Represents a request for a video based on a past event.
            /// </summary>
            Past,

            /// <summary>
            /// Represents a request for a video intended for a future event.
            /// </summary>
            Future
        }

        /// <summary>
        /// Defines the type of feed items displayed on the dashboard.
        /// </summary>
        public enum DashboardFeedType
        {
            /// <summary>
            /// Represents a video feed item.
            /// </summary>
            Video,

            /// <summary>
            /// Represents an event feed item.
            /// </summary>
            Event
        }

        /// <summary>
        /// Specifies the filter options for fetching the HappiFeed dashboard content.
        /// </summary>
        /// <remarks>
        /// Enum values:
        /// <list type="bullet">
        /// <item><description><c>All = 0</c> - Fetches all public feed items by default.</description></item>
        /// <item><description><c>Self = 1</c> - Fetches only the feed items created by the logged-in user.</description></item>
        /// <item><description><c>Following = 2</c> - Fetches feed items created by users that the logged-in user follows.</description></item>
        /// <item><description><c>Like = 3</c> - Fetches feed items that the logged-in user has liked.</description></item>

        /// </list>
        /// </remarks>
        public enum HappiFeedDashboardFilter
        {
            /// <summary>
            /// Fetches all public feed items by default.
            /// </summary>
            All,

            /// <summary>
            /// Fetches only the feed items created by the logged-in user.
            /// </summary>
            Self,

            /// <summary>
            /// Fetches feed items created by users that the logged-in user follows.
            /// </summary>
            Following,
            /// <summary>
            /// Fetches feed items that the logged-in user has liked.
            /// </summary>
            Like
        }

        public enum UserStatus
        {
            InActive,
            Active,
            Provisioned

        }

        public enum EventInviteSourceType
        {
            InApp,
            RegistrationPortal
        }

        public enum LanguageCode
        {
            /// <summary>English</summary>
            EN,

            /// <summary>Hindi</summary>
            HI,

            /// <summary>Spanish</summary>
            ES,

            /// <summary>French</summary>
            FR,

            /// <summary>German</summary>
            DE,

            /// <summary>Chinese (Simplified)</summary>
            ZH,

            /// <summary>Japanese</summary>
            JA,

            /// <summary>Korean</summary>
            KO,

            /// <summary>Arabic</summary>
            AR,

            /// <summary>Portuguese</summary>
            PT,

            /// <summary>Russian</summary>
            RU,

            /// <summary>Italian</summary>
            IT,

            /// <summary>Bengali</summary>
            BN,

            /// <summary>Urdu</summary>
            UR
        }


        /// <summary>
        /// Specifies the source context for generating OpenAI prompts in image analysis.
        /// Indicates whether the prompt is generated for a moment, an event, a video, or an image sequencing task.
        /// </summary>
        public enum ImageAnalyzerOpenAIPromptSource
        {
            /// <summary>
            /// The prompt is generated for an event created as a moment.
            /// </summary>
            CreateMoment,

            /// <summary>
            /// The prompt is generated for an event created as a full event.
            /// </summary>
            CreateEvent,

            /// <summary>
            /// The prompt is generated for an event created for a video.
            /// </summary>
            CreateVideo,

            /// <summary>
            /// The prompt is generated for image sequencing tasks.
            /// </summary>
            ImageSequencing
        }

        /// <summary>
        /// Event search categories
        /// </summary>
        public enum EventSearchCriteria
        {
            Recent,
            Trending,
            Popular,
            Recommended,
            FreeText
        }
    }
}
