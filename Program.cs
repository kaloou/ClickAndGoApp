using ClickAndCollect.DAL;
using ClickAndGoApp.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Session
builder.Services.AddSession(options =>//Activate the session 
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;//Session cookie isn't accessible from js
    options.Cookie.IsEssential = true;//Cookie works even if user refuses them
});

//DAL
builder.Services.AddScoped<DBConnection>();//A new instance is created by HTTP resuest
builder.Services.AddScoped<UserDAL>();
builder.Services.AddScoped<OrderPickerDAL>();
builder.Services.AddScoped<StoreDAL>();
builder.Services.AddScoped<OrderDAL>();
builder.Services.AddScoped<OrderLineDAL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

//Route by default is Login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();