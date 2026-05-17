using ClickAndGoApp.DAL;
using ClickAndGoApp.DAL.interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

//Session
builder.Services.AddSession(options => //Activate the session
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; //Session cookie isn't accessible from js
    options.Cookie.IsEssential = true; //Cookie works even if user refuses them
});

//Infrastructure
builder.Services.AddTransient<DBConnection>();

builder.Services.AddTransient<IProductDAL,           ProductDAL>();
builder.Services.AddTransient<ICategoryDAL,          CategoryDAL>();
builder.Services.AddTransient<IStoreDAL,             StoreDAL>();
builder.Services.AddTransient<IOrderDAL,             OrderDAL>();
builder.Services.AddTransient<IOrderLineDAL,         OrderLineDAL>();
builder.Services.AddTransient<ITimeSlotDAL,          TimeSlotDAL>();
builder.Services.AddTransient<IRecipeDAL,            RecipeDAL>();
builder.Services.AddTransient<IRecipeIngredientDAL,  RecipeIngredientDAL>();
builder.Services.AddTransient<ICustomerDAL,          CustomerDAL>();
builder.Services.AddTransient<UserDAL>();
builder.Services.AddTransient<IUserDAL,              UserDAL>();
builder.Services.AddTransient<IOrderPickerDAL,       OrderPickerDAL>();
builder.Services.AddTransient<ICashierDAL,           CashierDAL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
