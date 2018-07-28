using System;

namespace Core
{
    public class MessagePublished
    {
        public string Text { get; set; }
    }



    public class ScheduleNotification
    {
        public DateTime DeliveryTime { get; set; }
        public string Text { get; set; }
    }

    public class SendNotification
    {
        public string Text { get; set; }
    }
}
