using System.Text.Json;
using AspNetDeepDive.MiddleComponents;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

var builder = WebApplication.CreateBuilder();

builder.Services.AddTransient<MyCustomMiddleware>();
builder.Services.AddTransient<MyCustomExceptionHandler>();

var app = builder.Build();

app.UseMiddleware<MyCustomExceptionHandler>();

//Middleware 1
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    context.Response.Headers["MyHeader"] = "My header content";
   await context.Response.WriteAsync("Middleware #1: Before calling next\r\n");
    //context.Response.Headers["MyHeader"] = "My header content";
    await next(context);

    await context.Response.WriteAsync("Middleware #1: After calling next\r\n");
});

//app.MapWhen((context) =>
//{
//    return context.Request.Path.StartsWithSegments("/employees") &&
//            context.Request.Query.ContainsKey("id");
//},
//(appBuilder) =>
//{
//    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
//    {
//        await context.Response.WriteAsync("Middleware #7: Before calling next\r\n");

//        await next(context);

//        await context.Response.WriteAsync("Middleware #7: After calling next\r\n");
//    });
//});

//app.UseWhen((context) =>
//{
//    return context.Request.Path.StartsWithSegments("/employees") &&
//            context.Request.Query.ContainsKey("id");
//},
//(appBuilder) =>
//{
//    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
//    {
//        await context.Response.WriteAsync("Middleware #8: Before calling next\r\n");

//        await next(context);

//        await context.Response.WriteAsync("Middleware #8: After calling next\r\n");
//    });
//});

app.UseMiddleware<MyCustomMiddleware>();




app.Map("/employees", (appBuilder) =>
{
    
    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        throw new ApplicationException("Exception for testing.");
        await context.Response.WriteAsync("Middleware #5: Before calling next\r\n");

        await next(context);

        await context.Response.WriteAsync("Middleware #5: After calling next\r\n");
    });

    appBuilder.Use(async (HttpContext context, RequestDelegate next) =>
    {
        await context.Response.WriteAsync("Middleware #6: Before calling next\r\n");

        await next(context);

        await context.Response.WriteAsync("Middleware #6: After calling next\r\n");
    });
});
//MIddleware 2
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("Middleware #2: Before calling next\r\n");

    await next(context);

    await context.Response.WriteAsync("Middleware #2: After calling next\r\n");
});
//Middleware 3
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("Middleware #3: Before calling next\r\n");

    await next(context);

    await context.Response.WriteAsync("Middleware #3: After calling next\r\n");
});
//app.MapGet("/", () => "Hello World!");

app.Run(async (HttpContext context)=>
{
    await context.Response.WriteAsync("Middleware #4: Processed.\r\n");
});

app.Run(async (HttpContext context) =>
{
    if (context.Request.Method == "GET")
    {
        if (context.Request.Path.StartsWithSegments("/"))
        {
            await context.Response.WriteAsync($"The method is : {context.Request.Method}\r\n");
            await context.Response.WriteAsync($"The Url is: {context.Request.Path}\r\n");

            await context.Response.WriteAsync($"\r\nHeaders:\r\n");
            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"{key}: {context.Request.Headers[key]}\r\n");
            }
        }
        else if (context.Request.Path.StartsWithSegments("/employees"))
        {
            var employees = EmployessRepository.GetEmployees();
            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"{employee.Id} : {employee.Name} : {employee.Position}\r\n");
            }
        }
    }
    else if (context.Request.Method == "POST")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            EmployessRepository.AddEmployee(employee);
            await context.Response.WriteAsync("Employee is added successfully.");
        }
    }
    else if (context.Request.Method == "PUT")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            var result = EmployessRepository.UpdateEmployee(employee);

            if (result)
            {
                await context.Response.WriteAsync("Employee updated successfully.");
            }
            else
            {
                await context.Response.WriteAsync("Employee not found.");
            }
        }
    }
    else if (context.Request.Method == "DELETE")
    {
        if (context.Request.Path.StartsWithSegments("/employees"))
        {
            if (context.Request.Query.ContainsKey("id"))
            {
                var id = context.Request.Query["id"];
                if (int.TryParse(id, out int employeeId))
                {
                    if (context.Request.Headers["Authorization"] == "frank")
                    {
                        var result = EmployessRepository.DeleteEmployee(employeeId);
                        if (result)
                        {
                            await context.Response.WriteAsync("Employee is deleted successfully.");
                        }
                        else
                        {
                            await context.Response.WriteAsync("Employee not found.");
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync("You are not authorized to delete.");
                    }
                }
            }
        }
    }
});
app.Run();

static class EmployessRepository
{
    private static List<Employee> employees = new List<Employee>
    {
        new Employee ( 1,"John Doe", "Engineer", 60000),
        new Employee ( 2,"Nane Smith", "Manager", 75000),
        new Employee ( 3,"Sam Brown", "Technician", 50000)

    };

    public static List<Employee> GetEmployees() => employees;

    public static void AddEmployee(Employee? employee)
    {
        if (employee != null)
        {
            employees.Add(employee);
        }


    }

    public static bool UpdateEmployee(Employee? employee)
    {
        if (employee != null)
        {
            var emp = employees.FirstOrDefault(x => x.Id == employee.Id);
            if (emp != null)
            {
                emp.Name = employee.Name;
                emp.Position = employee.Position;
                emp.Salary = employee.Salary;
                return true;
            }

        }
        return false;
    }

    public static bool DeleteEmployee(int id)
    {
        var emloyee = employees.FirstOrDefault(x =>x.Id == id);
        if(emloyee != null)
        {
            employees.Remove(emloyee);
            return true;
        }
        return false;
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public double Salary { get; set; }

    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}