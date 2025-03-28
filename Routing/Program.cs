using System.Text.Json;
using Routing.Models;
using Routing.Repository;

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
        //context.Response.StatusCode = 200;

        var employees = EmployeesRepository.GetEmployees();


        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<h2>Employees</h2><br/>");
        await context.Response.WriteAsync("<ul>");
        foreach (var employee in employees)
        {
            await context.Response.WriteAsync($"<li><b>{employee.Id}</b> : {employee.Name} : {employee.Position}</li>");
        }
        await context.Response.WriteAsync("</ul>");
    });

    endpoints.MapGet("/employees/{id:int}", async (HttpContext context) =>
    {
        var id = context.Request.RouteValues["id"];
        var employeeId = int.Parse(id.ToString());


        var result = EmployeesRepository.GetEmployeeById(employeeId);
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<h2>Employees</h2><br/>");
        if (result != null)
        {

            await context.Response.WriteAsync($"Name:{result.Name}</br>");
            await context.Response.WriteAsync($"Name:{result.Position}</br>");
            await context.Response.WriteAsync($"Name:{result.Salary}</br>");
        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Employee not found.");
        }



    });
    endpoints.MapPost("/employees", async (HttpContext context) =>
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {

            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            EmployeesRepository.AddEmployee(employee);

            context.Response.StatusCode = 201;
            await context.Response.WriteAsync("Employee is added successfully.");
        }
    });
    endpoints.MapPut("/employees", async (HttpContext context) =>
    {
        await context.Response.WriteAsync("Update an employees");
    });
    endpoints.MapDelete("/employees/{id:int}", async (HttpContext context) =>
    {
        
       
           
                var id = context.Request.RouteValues["id"];
                var employeeId = int.Parse(id.ToString());


                var result = EmployeesRepository.DeleteEmployee(employeeId);


                if (result != null)
                {
                    await context.Response.WriteAsync("Employee is deleted successfully.");
                    await context.Response.WriteAsync($"Delete the employees: {context.Request.RouteValues["id"]}");
                }
                else
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Employee not found.");
                }
            
        
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
