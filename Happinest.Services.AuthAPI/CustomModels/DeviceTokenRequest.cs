    namespace Happinest.Services.AuthAPI.CustomModels
{
    public class DeviceTokenRequest
    {
        public string Token { get; set; }
    }

    public class SendPushNotificationRequest
    {
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public int NotificationTypeId { get; set; }
        public long eventId { get; set; }
        //public string ActionScreen { get; set; }
    }
    public class SendPushNotificationResponse : BaseResponse
    {

    }
}
