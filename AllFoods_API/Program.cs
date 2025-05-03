using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.MappingCofig;
using AllFoods.Core.ServiceContracts.ICategoriesService;
using AllFoods.Core.ServiceContracts.IProductsService;
using AllFoods.Core.ServiceContracts.IUsersService;
using AllFoods.Core.Services.CategoriesService;
using AllFoods.Core.Services.ProductsService;
using AllFoods.Core.Services.UsersService;
using AllFoods.Infrastructure.DbContext;
using AllFoods.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using AllFoods_API.StartupExtensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using AllFoods.Core.ServiceContracts.ICartsService;
using AllFoods.Core.Services.CartsService;
using AllFoods.Core.ServiceContracts.IOrdersService;
using AllFoods.Core.Services.OrdersService;
using AllFoods.Core.ServiceContracts;
using AllFoods.Core.Services;
using AllFoods_API.Middleware;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new ArgumentNullException("problem with connection string");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Add Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//var key = builder.Configuration.GetValue<string>("APISettings:Secret");

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<ICacheService,RedisCacheService>();

builder.Services.AddScoped<IProductsGetterService, ProductsGetterService>();
builder.Services.AddScoped<IProductsAdderService, ProductsAdderService>();
builder.Services.AddScoped<IProductsUpdaterService, ProductsUpdaterService>();
builder.Services.AddScoped<IProductsDeleterService, ProductsDeleterService>();
builder.Services.AddScoped<IProductsSorterService, ProductsSorterService>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();


builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<ICategoriesGetterService, CategoriesGetterService>();
builder.Services.AddScoped<ICategoriesAdderService, CategoriesAdderService>();
builder.Services.AddScoped<ICategoriesDeleterService, CategoriesDeleterService>();
builder.Services.AddScoped<ICategoriesUpdaterService, CategoriesUpdaterService>();

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();


builder.Services.AddScoped<ICartsAdderService, CartsAdderService>();
builder.Services.AddScoped<ICartsGetterService, CartsGetterService>();
builder.Services.AddScoped<ICartsDeleterService, CartsDeleterService>();

builder.Services.AddScoped<ICartsRepository, CartsRepository>();

builder.Services.AddScoped<ICartItemsRepository, CartItemsRepository>();


builder.Services.AddScoped<IUsersGetterService, UsersGetterService>();
builder.Services.AddScoped<IUsersDeleterService, UsersDeleterService>();
builder.Services.AddScoped<IUsersUpdaterService, UsersUpdaterService>();
builder.Services.AddScoped<IUsersLockerService, UsersLockerService>();


builder.Services.AddScoped<IOrdersAdderService, OrdersAdderService>();
builder.Services.AddScoped<IOrdersGetterService, OrdersGetterService>();
builder.Services.AddScoped <IOrdersDeleterService, OrdersDeleterService>();

builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();


builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddAutoMapper(typeof(MappingConfig));


var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // to read version from url route
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;

});

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g., v1, v2
    options.SubstituteApiVersionInUrl = true;
});


var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("APISettings:Secret").ToString()!);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Enabled to use token authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        //ValidIssuer = "https://allfood-api.com",
        //ValidAudience = "https://test-allfood-api.com",
        ClockSkew = TimeSpan.Zero // if token expired one second ago, it will be not valid token and API not accept that
    };
}
    );


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Disable auto-400
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
//app.UseErrorHandlingMiddleware();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger(); // creates endpoints for swagger.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    }); // creates swagger UI for testing all API endpoints / action methods
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if(db.Database.GetPendingMigrations().Count() > 0)
        {
            db.Database.Migrate();
        }
    }
}
