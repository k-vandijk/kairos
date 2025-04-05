namespace kairos_api.DTOs.CapsuleDTOs;

public class CreateCapsuleDTO
{
    public string Content { get; set; }
    public DateTime DateToOpen { get; set; }
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
}
