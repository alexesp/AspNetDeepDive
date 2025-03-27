var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
//app.MapGet("/employees", async (HttpContext context) =>
//{
//    await context.Response.WriteAsync("Get employees");
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/employees", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Get employees");
});
    endpoints.MapPost("/employees", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Create employees");
    });
    endpoints.MapPut("/employees", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Update employees");
    });
    endpoints.MapDelete("/employees", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Delete employees");
    });
});

app.Run();
