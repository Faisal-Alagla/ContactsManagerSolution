using Serilog;
using CRUD_Example;
using CRUD_Example.Middleware;
using ContactsManager.Core.ServiceContracts.PersonsServiceContracts;
using ContactsManager.UI.Filters.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    //adding a global filter
    //if we're not supplying any parameters to the filter class, we can use this and provide the order as following
    //options.Filters.Add<ResponseHeaderActionFilter>(5);

    //var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

    //order = 2 (IOrderedFilter)
    options.Filters.Add(new ResponseHeaderActionFilter(/*logger,*/ "Some-Key", "Some-Value", 2));
});

//logging with serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //assign same logging configuration into serilog (from appsettings)
    .ReadFrom.Services(services); //reads the services and makes them available to the serilog
});

builder.Services.ConfigureServices(builder.Configuration); //this contains all the commented services below (check StartupExtensions)

//builder.Services.AddHttpLogging(options =>
//{
//    //http logging options here
//    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties;
//    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
//});

////Configure services extension
////adding services into IoC container
//builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
//builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
//builder.Services.AddScoped<ICountriesService, CountriesService>();
//builder.Services.AddScoped<IPersonsService, PersonsService>();

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    {
//        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//    });

////In case of using ServiceFilter instead of TypeFilter
////Here can decide transient / scoped / singleton.. TypeFilter is Transient by default
////builder.Services.AddTransient<TokenResultFilter>();

////Have to do this for IFilterFactory (check its file)
//builder.Services.AddTransient<PersonsListResultFilter>();

//build
var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //For redirection to specific route in case of exceptions. here redirecting to /Error
    app.UseExceptionHandler("/Error");

    //in other environments we use our custom exception handling middleware (in Middleware file)
    app.UseExceptionHandlingMiddleware();
}

if (!builder.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseHsts(); //forces https
app.UseHttpsRedirection(); //enable https

app.UseHttpLogging();
app.UseStaticFiles();

//the following must be in the same order:
app.UseRouting(); //identifying action method based on route
app.UseAuthentication(); //reading identity cookie
app.UseAuthorization(); //access permissions of the user
app.MapControllers(); //execute filter pipeline (action methods + filters)

//Attribute routing is what's done on controllers to define how to reach each one (the default)
//the following is Conventional routing (lec. 321)
app.UseEndpoints(endpoints =>
{
    //specific to areas
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}" //here: ":exists" means it's a mandatory parameter (area here)
                                                                  //"=..." is default .. e.g. /Admin will route to /Admin/Home/Index
        );

    //general
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}"
        );
});
app.Run();

//making it partial so that we can access the program class anywhwere in the application (e.g. integration tests)
//+ have to change csproj file so that it's available in other projects within the same solution (edit project file)
public partial class Program { }