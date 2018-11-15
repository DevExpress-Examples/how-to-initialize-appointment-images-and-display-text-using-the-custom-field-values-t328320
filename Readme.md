<!-- default file list -->
*Files to look at*:

* [CustomObjects.cs](./CS/CustomAppointmentImageAndText/CustomObjects.cs) (VB: [CustomObjects.vb](./VB/CustomAppointmentImageAndText/CustomObjects.vb))
* [Form1.cs](./CS/CustomAppointmentImageAndText/Form1.cs) (VB: [Form1.vb](./VB/CustomAppointmentImageAndText/Form1.vb))
<!-- default file list end -->
# How to initialize appointment images and display text using the custom field values


<p>Starting with version 15.2, we split layout calculation operations into several threads to improve the typical performance of a Scheduling application. Correspondingly, in a common scenario, the <strong>InitAppointmentImages</strong> event (as well as <strong>InitAppointmentDisplayText</strong>) can be raised from another thread than the one where the SchedulerControl instance was created. Accessing the SchedulerControl (SchedulerStorage) instance directly in the InitAppointmentImages event when the <strong>OptionsBehavior.UseAsyncMode</strong> property is true can result in "cross-thread" exceptions.<br><br>Correspondingly, to avoid the "cross-thread" issues in a scenario when the <strong>OptionsBehavior.UseAsyncMode</strong> property is true, you need to avoid direct accessing the SchedulerControl or SchedulerStorage instance in the mentioned event handlers.<br>To implement it, we recommend storing all the required information to calculate custom appointment images and the display text as custom appointment fields, to operate only with the current appointment instance without accessing an underlying source object.<br>This example demonstrates how it can be achieved.</p>

<br/>


