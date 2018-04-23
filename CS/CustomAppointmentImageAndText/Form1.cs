using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;

namespace CustomAppointmentImageAndText {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            //this.schedulerStorage1.Resources.ColorSaving = ColorSavingType.Color;
            schedulerControl1.InitAppointmentDisplayText += schedulerControl1_InitAppointmentDisplayText;
            schedulerControl1.InitAppointmentImages += schedulerControl1_InitAppointmentImages;
        }

        public static Random RandomInstance = new Random();
        public string ImagePath = Application.ExecutablePath + "\\Images";

        private List<CustomResource> CustomResourceCollection = new List<CustomResource>();
        private List<CustomAppointment> CustomEventList = new List<CustomAppointment>();

        private void Form1_Load(object sender, EventArgs e) {
            InitResources();
            InitAppointments();

            schedulerStorage1.BeginUpdate();
            try {
                schedulerStorage1.Resources.DataSource = CustomResourceCollection;
                schedulerStorage1.Appointments.DataSource = CustomEventList;
        }
            finally {
                schedulerStorage1.EndUpdate();
            }

            schedulerControl1.Start = DateTime.Now.AddDays(-5);
            schedulerControl1.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource;

            schedulerControl1.TimelineView.Scales.Clear();
            schedulerControl1.TimelineView.Scales.Add(new TimeScaleDay());
            schedulerControl1.TimelineView.Scales.Add(new TimeScaleHour());

            schedulerControl1.TimelineView.AppointmentDisplayOptions.AppointmentAutoHeight = true;

            schedulerControl1.ActiveViewType = SchedulerViewType.Day;
        }

        private void InitResources() {
            ResourceMappingInfo mappings = this.schedulerStorage1.Resources.Mappings;
            mappings.Id = "ResID";
            mappings.Caption = "Name";
            mappings.Color = "ResColor";

            CustomResourceCollection.Add(CreateCustomResource(1, "Max Fowler", Color.PowderBlue));
            CustomResourceCollection.Add(CreateCustomResource(2, "Nancy Drewmore", Color.PaleVioletRed));
            CustomResourceCollection.Add(CreateCustomResource(3, "Pak Jang", Color.PeachPuff));
        }

        private CustomResource CreateCustomResource(int res_id, string caption, Color resColor) {
            CustomResource cr = new CustomResource();
            cr.ResID = res_id;
            cr.Name = caption;
            cr.ResColor = resColor;
            return cr;
        }


        #region #initappointments
        private void InitAppointments() {
            AppointmentMappingInfo mappings = this.schedulerStorage1.Appointments.Mappings;
            mappings.Start = "StartTime";
            mappings.End = "EndTime";
            mappings.Subject = "Subject";
            mappings.AllDay = "AllDay";
            mappings.Description = "Description";
            mappings.Label = "Label";
            mappings.Location = "Location";
            mappings.RecurrenceInfo = "RecurrenceInfo";
            mappings.ReminderInfo = "ReminderInfo";
            mappings.ResourceId = "OwnerId";
            mappings.Status = "Status";
            mappings.Type = "EventType";            

            schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("ApptImage1", "Icon1", FieldValueType.Object));
            schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("ApptImage2", "Icon2", FieldValueType.Object));
            schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("ApptAddInfo", "AdditionalInfo", FieldValueType.String));

            GenerateEvents(CustomEventList, 3);
        }
        #endregion #initappointments


        private void GenerateEvents(List<CustomAppointment> eventList, int count) {

            for(int i = 0; i < count; i++) {
                CustomResource c_Resource = CustomResourceCollection[i];
                string subjPrefix = c_Resource.Name + "'s ";
                Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();

                eventList.Add(CreateEvent(subjPrefix + "meeting", "The meeting will be held in the Conference Room", c_Resource.ResID, 2, 5, 14, currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.BOCustomer_16x16.png"), currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.Project_32x32.png")));
                eventList.Add(CreateEvent(subjPrefix + "travel", "Book a hotel in advance", c_Resource.ResID, 3, 6, 10, currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.Country_16x16.png"), currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.BOChangeHistory_32x32.png")));
                eventList.Add(CreateEvent(subjPrefix + "phone call", "Important phone call", c_Resource.ResID, 0, 4, 16, currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.BOContact_16x16.png"), currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.EditTask_32x32.png")));
            }
        }
        private CustomAppointment CreateEvent(string subject, string additionalInfo, object resourceId, int status, int label, int sHour, Stream icon1, Stream icon2) {
            CustomAppointment apt = new CustomAppointment();
            apt.Subject = subject;
            apt.OwnerId = resourceId;
            Random rnd = RandomInstance;

            apt.StartTime = DateTime.Today.AddHours(sHour);
            apt.EndTime = apt.StartTime.AddHours(3);
            apt.Status = status;
            apt.Label = label;

            using(MemoryStream ms = new MemoryStream()) {
                icon1.CopyTo(ms);
                apt.Icon1 = ms.ToArray();
            }
            using(MemoryStream ms = new MemoryStream()) {
                icon2.CopyTo(ms);
                apt.Icon2 = ms.ToArray();
            }

            apt.AdditionalInfo = additionalInfo;
            return apt;
        }
        #region #initappointmentimages
        private void schedulerControl1_InitAppointmentImages(object sender, AppointmentImagesEventArgs e) {
            if (e.Appointment.CustomFields["ApptImage1"] != null) {
                byte[] imageBytes = (byte[])e.Appointment.CustomFields["ApptImage1"];
                if (imageBytes != null) {
                    AppointmentImageInfo info = new AppointmentImageInfo();
                    using (MemoryStream ms = new MemoryStream(imageBytes)) {
                        info.Image = Image.FromStream(ms);
                        e.ImageInfoList.Add(info);
                    }
                }
            }

            if (e.Appointment.CustomFields["ApptImage2"] != null) {
                byte[] imageBytes = (byte[])e.Appointment.CustomFields["ApptImage2"];
                if (imageBytes != null) {
                    AppointmentImageInfo info = new AppointmentImageInfo();
                    using (MemoryStream ms = new MemoryStream(imageBytes)) {
                        info.Image = Image.FromStream(ms);
                        e.ImageInfoList.Add(info);
                    }
                }
            }
        }
        #endregion #initappointmentimages

        #region #initappointmentdisplaytext
        private void schedulerControl1_InitAppointmentDisplayText(object sender, AppointmentDisplayTextEventArgs e) {
            // Display custom text in Day and WorkWeek views only (VerticalAppointmentViewInfo).
            if (e.ViewInfo is VerticalAppointmentViewInfo && e.Appointment.CustomFields["ApptAddInfo"] != null) {
                e.Text = e.Appointment.Subject + "\r\n";
                e.Text += "------\r\n";
                e.Text += e.Appointment.CustomFields["ApptAddInfo"].ToString();
            }
        }
        #endregion #initappointmentdisplaytext
    }
}
