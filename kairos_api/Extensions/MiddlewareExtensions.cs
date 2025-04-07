namespace kairos_api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder ConfigureMiddleware(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler();

        return app;
    }
}
