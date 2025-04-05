using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace kairos_api.Entities;

public class Capsule : BaseEntity
{
    [ForeignKey("UserId")]
    [ValidateNever]
    public User User { get; set; }
    public Guid UserId { get; set; }

    public string Content { get; set; }
    public DateTime DateToOpen { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
