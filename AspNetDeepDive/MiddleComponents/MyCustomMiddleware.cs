
namespace AspNetDeepDive.MiddleComponents
{
    public class MyCustomMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            
                //context.Response.Headers["MyHeader"] = "My header content";
                await context.Response.WriteAsync("My custom Middleware #: Before calling next\r\n");
                //context.Response.Headers["MyHeader"] = "My header content";
                await next(context);

                await context.Response.WriteAsync("My custom Middleware #: After calling next\r\n");
           
        }
    }
}
