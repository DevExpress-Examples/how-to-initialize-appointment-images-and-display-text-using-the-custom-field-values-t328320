using System;

namespace CustomAppointmentImageAndText {
    #region #customappointment
    public class CustomAppointment {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }
        public object OwnerId { get; set; }

        public byte[] Icon1 { get; set; }
        public byte[] Icon2 { get; set; }

        public string AdditionalInfo { get; set; }
    }
    #endregion  #customappointment

    #region #customresource
    public class CustomResource {
        public string Name { get; set; }
        public int ResID { get; set; }
        public System.Drawing.Color ResColor { get; set; }
        // To display resources using the specified color set the SchedulerStorage.Resources.ColorSaving property to ColorSavingType.Color.
    }
    #endregion #customresource
}
