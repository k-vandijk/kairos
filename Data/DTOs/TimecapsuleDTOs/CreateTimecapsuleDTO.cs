namespace Data.DTOs.TimecapsuleDTOs;

public class CreateTimecapsuleDTO
{
    public string Content { get; set; }
    public DateTime DateToOpen { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
}
