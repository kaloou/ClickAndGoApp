using ClickAndGoApp.DAL;

var builder = WebApplication.CreateBuilder(args);

// Register MVC with session-based TempData instead of the default cookie provider.
// This keeps TempData in the server-side session rather than a cookie, which is safer for larger payloads.
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

// Session configuration
builder.Services.AddSession(options =>
{
    // IdleTimeout: the server discards session data after 30 minutes of inactivity.
    // This timer resets on every request, so active users stay logged in indefinitely.
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    // HttpOnly prevents JavaScript from reading the session cookie — protects against XSS.
    options.Cookie.HttpOnly = true;
    // IsEssential marks the cookie as necessary so it is set even without cookie consent.
    options.Cookie.IsEssential = true;
    // MaxAge makes the session cookie persistent in the browser across restarts.
    // Without this, the browser deletes the cookie when closed and the user appears logged out
    // even though the server-side session is still alive.
    options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
});

// DBConnection is registered as a Singleton because it only holds a connection string —
// one instance shared across all requests is both safe and efficient.
builder.Services.AddSingleton(sp =>
    DBConnection.GetInstance(sp.GetRequiredService<IConfiguration>()));

// DALs are registered as Transient — a new instance is created per injection.
// They hold no state, so transient lifetime is safe and avoids shared-state issues.
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

app.UseExceptionHandler("/Home/Error");
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// UseSession must be called before MapControllerRoute so the session is available in every action.
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
