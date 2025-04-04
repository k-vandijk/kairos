namespace kairos_api.DTOs.TimecapsuleDTOs;

public class TimecapsuleDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public DateTime DateToOpen { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
