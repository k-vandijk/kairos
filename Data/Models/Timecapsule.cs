using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Timecapsule : BaseModel
{
    [ForeignKey("UserId")]
    [ValidateNever]
    public ApplicationUser User { get; set; }
    public Guid UserId { get; set; }

    public string Content { get; set; }
    public DateTime DateToOpen { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
