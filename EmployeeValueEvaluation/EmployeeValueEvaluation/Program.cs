using EmployeeValueEvaluation.Data;
using EmployeeValueEvaluation.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// تنظیمات مدل ارزندگی (قابل بایند از بخش "Evaluation" در appsettings.json)
var evaluationSettings = new EvaluationSettings();
builder.Configuration.GetSection("Evaluation").Bind(evaluationSettings);
builder.Services.AddSingleton(evaluationSettings);

// موتور محاسبه ارزندگی
builder.Services.AddScoped<IValueEvaluationService, ValueEvaluationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
