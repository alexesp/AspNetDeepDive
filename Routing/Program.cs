var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
//app.MapGet("/employees", async (HttpContext context) =>
//{
//    await context.Response.WriteAsync("Get employees");
//});

//app.Use(async (context, next) =>
//{
//    await next(context);
//});

app.UseRouting();

//app.Use(async (context, next) =>
//{
//    await next(context);
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Welcome to the home page.");
    });
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
        await context.Response.WriteAsync("Update an employees");
    });
    endpoints.MapDelete("/employees/{id:int}", async (HttpContext context) =>
    {
        await context.Response.WriteAsync($"Delete the employees: {context.Request.RouteValues["id"]}");
    });
    //endpoints.MapGet("/{category=shirts}/{size=medium}/{id=0}", async (HttpContext context) =>
    //{
    //    await context.Response.WriteAsync($"Get categories: {context.Request.RouteValues["category"]} in size: {context.Request.RouteValues["size"]}");
    //});
    //endpoints.MapGet("/{category=shirts}/{size=medium}/{id?}", async (HttpContext context) =>
    //{
    //    await context.Response.WriteAsync($"Get categories: {context.Request.RouteValues["category"]} in size: {context.Request.RouteValues["size"]}");
    //});
});


app.Run();
