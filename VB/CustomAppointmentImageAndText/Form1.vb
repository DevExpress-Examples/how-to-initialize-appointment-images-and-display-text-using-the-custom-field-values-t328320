Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Drawing

Namespace CustomAppointmentImageAndText
    Partial Public Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
            'this.schedulerStorage1.Resources.ColorSaving = ColorSavingType.Color;
            AddHandler schedulerControl1.InitAppointmentDisplayText, AddressOf schedulerControl1_InitAppointmentDisplayText
            AddHandler schedulerControl1.InitAppointmentImages, AddressOf schedulerControl1_InitAppointmentImages
        End Sub

        Public Shared RandomInstance As New Random()
        Public ImagePath As String = Application.ExecutablePath & "\Images"

        Private CustomResourceCollection As New List(Of CustomResource)()
        Private CustomEventList As New List(Of CustomAppointment)()

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            InitResources()
            InitAppointments()

            schedulerStorage1.BeginUpdate()
            Try
                schedulerStorage1.Resources.DataSource = CustomResourceCollection
                schedulerStorage1.Appointments.DataSource = CustomEventList
            Finally
                schedulerStorage1.EndUpdate()
            End Try

            schedulerControl1.Start = Date.Now.AddDays(-5)
            schedulerControl1.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource

            schedulerControl1.TimelineView.Scales.Clear()
            schedulerControl1.TimelineView.Scales.Add(New TimeScaleDay())
            schedulerControl1.TimelineView.Scales.Add(New TimeScaleHour())

            schedulerControl1.TimelineView.AppointmentDisplayOptions.AppointmentAutoHeight = True

            schedulerControl1.ActiveViewType = SchedulerViewType.Day
        End Sub

        Private Sub InitResources()
            Dim mappings As ResourceMappingInfo = Me.schedulerStorage1.Resources.Mappings
            mappings.Id = "ResID"
            mappings.Caption = "Name"
            mappings.Color = "ResColor"

            CustomResourceCollection.Add(CreateCustomResource(1, "Max Fowler", Color.PowderBlue))
            CustomResourceCollection.Add(CreateCustomResource(2, "Nancy Drewmore", Color.PaleVioletRed))
            CustomResourceCollection.Add(CreateCustomResource(3, "Pak Jang", Color.PeachPuff))
        End Sub

        Private Function CreateCustomResource(ByVal res_id As Integer, ByVal caption As String, ByVal resColor As Color) As CustomResource
            Dim cr As New CustomResource()
            cr.ResID = res_id
            cr.Name = caption
            cr.ResColor = resColor
            Return cr
        End Function


        #Region "#initappointments"
        Private Sub InitAppointments()
            Dim mappings As AppointmentMappingInfo = Me.schedulerStorage1.Appointments.Mappings
            mappings.Start = "StartTime"
            mappings.End = "EndTime"
            mappings.Subject = "Subject"
            mappings.AllDay = "AllDay"
            mappings.Description = "Description"
            mappings.Label = "Label"
            mappings.Location = "Location"
            mappings.RecurrenceInfo = "RecurrenceInfo"
            mappings.ReminderInfo = "ReminderInfo"
            mappings.ResourceId = "OwnerId"
            mappings.Status = "Status"
            mappings.Type = "EventType"

            schedulerStorage1.Appointments.CustomFieldMappings.Add(New AppointmentCustomFieldMapping("ApptImage1", "Icon1", FieldValueType.Object))
            schedulerStorage1.Appointments.CustomFieldMappings.Add(New AppointmentCustomFieldMapping("ApptImage2", "Icon2", FieldValueType.Object))
            schedulerStorage1.Appointments.CustomFieldMappings.Add(New AppointmentCustomFieldMapping("ApptAddInfo", "AdditionalInfo", FieldValueType.String))

            GenerateEvents(CustomEventList, 3)
        End Sub
        #End Region ' #initappointments


        Private Sub GenerateEvents(ByVal eventList As List(Of CustomAppointment), ByVal count As Integer)

            For i As Integer = 0 To count - 1
                Dim c_Resource As CustomResource = CustomResourceCollection(i)
                Dim subjPrefix As String = c_Resource.Name & "'s "
                Dim currentAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()

                eventList.Add(CreateEvent(subjPrefix & "meeting", "The meeting will be held in the Conference Room", c_Resource.ResID, 2, 5, 14, currentAssembly.GetManifestResourceStream("Images.BOCustomer_16x16.png"), currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.Project_32x32.png")))
                eventList.Add(CreateEvent(subjPrefix & "travel", "Book a hotel in advance", c_Resource.ResID, 3, 6, 10, currentAssembly.GetManifestResourceStream("Images.Country_16x16.png"), currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.BOChangeHistory_32x32.png")))
                eventList.Add(CreateEvent(subjPrefix & "phone call", "Important phone call", c_Resource.ResID, 0, 4, 16, currentAssembly.GetManifestResourceStream("Images.BOContact_16x16.png"), currentAssembly.GetManifestResourceStream("CustomAppointmentImageAndText.Images.EditTask_32x32.png")))
            Next i
        End Sub
        Private Function CreateEvent(ByVal subject As String, ByVal additionalInfo As String, ByVal resourceId As Object, ByVal status As Integer, ByVal label As Integer, ByVal sHour As Integer, ByVal icon1 As Stream, ByVal icon2 As Stream) As CustomAppointment
            Dim apt As New CustomAppointment()
            apt.Subject = subject
            apt.OwnerId = resourceId
            Dim rnd As Random = RandomInstance

            apt.StartTime = Date.Today.AddHours(sHour)
            apt.EndTime = apt.StartTime.AddHours(3)
            apt.Status = status
            apt.Label = label

            Using ms As New MemoryStream()
                icon1.CopyTo(ms)
                apt.Icon1 = ms.ToArray()
            End Using
            Using ms As New MemoryStream()
                icon2.CopyTo(ms)
                apt.Icon2 = ms.ToArray()
            End Using

            apt.AdditionalInfo = additionalInfo
            Return apt
        End Function
        #Region "#initappointmentimages"
        Private Sub schedulerControl1_InitAppointmentImages(ByVal sender As Object, ByVal e As AppointmentImagesEventArgs)
            If e.Appointment.CustomFields("ApptImage1") IsNot Nothing Then
                Dim imageBytes() As Byte = CType(e.Appointment.CustomFields("ApptImage1"), Byte())
                If imageBytes IsNot Nothing Then
                    Dim info As New AppointmentImageInfo()
                    Using ms As New MemoryStream(imageBytes)
                        info.Image = Image.FromStream(ms)
                        e.ImageInfoList.Add(info)
                    End Using
                End If
            End If

            If e.Appointment.CustomFields("ApptImage2") IsNot Nothing Then
                Dim imageBytes() As Byte = CType(e.Appointment.CustomFields("ApptImage2"), Byte())
                If imageBytes IsNot Nothing Then
                    Dim info As New AppointmentImageInfo()
                    Using ms As New MemoryStream(imageBytes)
                        info.Image = Image.FromStream(ms)
                        e.ImageInfoList.Add(info)
                    End Using
                End If
            End If
        End Sub
        #End Region ' #initappointmentimages

        #Region "#initappointmentdisplaytext"
        Private Sub schedulerControl1_InitAppointmentDisplayText(ByVal sender As Object, ByVal e As AppointmentDisplayTextEventArgs)
            ' Display custom text in Day and WorkWeek views only (VerticalAppointmentViewInfo).
            If TypeOf e.ViewInfo Is VerticalAppointmentViewInfo AndAlso e.Appointment.CustomFields("ApptAddInfo") IsNot Nothing Then
                e.Text = e.Appointment.Subject & ControlChars.CrLf
                e.Text &= "------" & ControlChars.CrLf
                e.Text += e.Appointment.CustomFields("ApptAddInfo").ToString()
            End If
        End Sub
        #End Region ' #initappointmentdisplaytext
    End Class
End Namespace
