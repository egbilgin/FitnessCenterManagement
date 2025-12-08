using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

public class TrainerFormViewModel
{
    public Trainer Trainer { get; set; }

    // Hizmet checkbox list için
    public List<SelectListItem> ServiceTypes { get; set; } = new();

    public List<int> SelectedServiceIds { get; set; } = new();

    // Haftalık availability için
    public List<TrainerAvailability> WeeklyAvailability { get; set; } = new();

    public List<string> DaysOfWeek { get; set; } = new()
    {
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
    };
}
