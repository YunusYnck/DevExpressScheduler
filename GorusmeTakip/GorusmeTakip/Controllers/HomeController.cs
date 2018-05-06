using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GorusmeTakip.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        GorusmeTakip.Models.GorusmeTkpModellerim appointmentContext = new GorusmeTakip.Models.GorusmeTkpModellerim();
        GorusmeTakip.Models.GorusmeTkpModellerim resourceContext = new GorusmeTakip.Models.GorusmeTkpModellerim();

        public ActionResult SchedulerPartial()
        {
            var appointments = appointmentContext.Appointments;
            var resources = resourceContext.Resources;

            ViewData["Appointments"] = appointments.ToList();
            ViewData["Resources"] = resources.ToList();

            return PartialView("_SchedulerPartial");
        }
        public ActionResult SchedulerPartialEditAppointment()
        {
            var appointments = appointmentContext.Appointments;
            var resources = resourceContext.Resources;

            try
            {
                HomeControllerSchedulerSettings.UpdateEditableDataObject(appointmentContext, resourceContext);
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            ViewData["Appointments"] = appointments.ToList();
            ViewData["Resources"] = resources.ToList();

            return PartialView("_SchedulerPartial");
        }
    }
    public class HomeControllerSchedulerSettings
    {
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "UniqueID";
                    appointmentStorage.Mappings.Start = "StartDate";
                    appointmentStorage.Mappings.End = "EndDate";
                    appointmentStorage.Mappings.Subject = "Subject";
                    appointmentStorage.Mappings.Description = "Description";
                    appointmentStorage.Mappings.Location = "Location";
                    appointmentStorage.Mappings.AllDay = "AllDay";
                    appointmentStorage.Mappings.Type = "Type";
                    appointmentStorage.Mappings.RecurrenceInfo = "RecurrenceInfo";
                    appointmentStorage.Mappings.ReminderInfo = "ReminderInfo";
                    appointmentStorage.Mappings.Label = "Label";
                    appointmentStorage.Mappings.Status = "Status";
                    appointmentStorage.Mappings.ResourceId = "ResourceID";
                }
                return appointmentStorage;
            }
        }

        static DevExpress.Web.Mvc.MVCxResourceStorage resourceStorage;
        public static DevExpress.Web.Mvc.MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new DevExpress.Web.Mvc.MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "ResourceID";
                    resourceStorage.Mappings.Caption = "CustomField1";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject(GorusmeTakip.Models.GorusmeTkpModellerim appointmentContext, GorusmeTakip.Models.GorusmeTkpModellerim resourceContext)
        {
            InsertAppointments(appointmentContext, resourceContext);
            UpdateAppointments(appointmentContext, resourceContext);
            DeleteAppointments(appointmentContext, resourceContext);
        }

        static void InsertAppointments(GorusmeTakip.Models.GorusmeTkpModellerim appointmentContext, GorusmeTakip.Models.GorusmeTkpModellerim resourceContext)
        {
            var appointments = appointmentContext.Appointments.ToList();
            var resources = resourceContext.Resources;

            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<GorusmeTakip.Models.Appointment>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                appointmentContext.Appointments.Add(appointment);
            }
            appointmentContext.SaveChanges();
        }
        static void UpdateAppointments(GorusmeTakip.Models.GorusmeTkpModellerim appointmentContext, GorusmeTakip.Models.GorusmeTkpModellerim resourceContext)
        {
            var appointments = appointmentContext.Appointments.ToList();
            var resources = resourceContext.Resources;

            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<GorusmeTakip.Models.Appointment>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in updAppointments)
            {
                var origAppointment = appointments.FirstOrDefault(a => a.UniqueID == appointment.UniqueID);
                appointmentContext.Entry(origAppointment).CurrentValues.SetValues(appointment);
            }
            appointmentContext.SaveChanges();
        }

        static void DeleteAppointments(GorusmeTakip.Models.GorusmeTkpModellerim appointmentContext, GorusmeTakip.Models.GorusmeTkpModellerim resourceContext)
        {
            var appointments = appointmentContext.Appointments.ToList();
            var resources = resourceContext.Resources;

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<GorusmeTakip.Models.Appointment>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                var delAppointment = appointments.FirstOrDefault(a => a.UniqueID == appointment.UniqueID);
                if (delAppointment != null)
                    appointmentContext.Appointments.Remove(delAppointment);
            }
            appointmentContext.SaveChanges();
        }
    }

}