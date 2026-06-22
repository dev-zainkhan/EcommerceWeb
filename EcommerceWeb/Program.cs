using EcommerceWeb.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Load data-access settings from configuration (appsettings.json /
// environment / user-secrets) instead of hard-coding secrets in source.
DbHelper.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
S3Helper.AccessKey = builder.Configuration["AWS:AccessKey"] ?? string.Empty;
S3Helper.SecretKey = builder.Configuration["AWS:SecretKey"] ?? string.Empty;
S3Helper.BucketName = builder.Configuration["AWS:BucketName"] ?? string.Empty;
S3Helper.Region = builder.Configuration["AWS:Region"] ?? "us-east-1";

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   
}


app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
