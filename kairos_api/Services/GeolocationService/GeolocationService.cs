namespace kairos_api.Services.GeolocationService;

public class GeolocationService : IGeolocationService
{
    private const double EarthRadius = 6371000;

    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Convert degrees to radians
        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(lat2);
        double lon2Rad = DegreesToRadians(lon2);

        // Differences
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // Haversine formula
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadius * c;

        return distance;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
