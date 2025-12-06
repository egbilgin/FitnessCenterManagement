using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessCenterManagement.Models.ViewModels
{
    public class AppointmentCreateViewModel
    {
        // Admin için dropdown ile seçilecek
        // Member için sistem otomatik dolduracak
        public int MemberId { get; set; }

        public int TrainerId { get; set; }
        public int ServiceTypeId { get; set; }

        public DateTime RequestedStartTime { get; set; }

        // Dropdown listeleri (Admin kullanacak)
        public List<SelectListItem> Members { get; set; } = new();
        public List<SelectListItem> Trainers { get; set; } = new();
        public List<SelectListItem> ServiceTypes { get; set; } = new();
    }
}
