//using RazorPageClient.Protos;

using RazorPageClient;

var builder = WebApplication.CreateBuilder(args);
// gRPC clients configuration
builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5001"); // Server URL (make sure the gRPC server is running on this address)
});

builder.Services.AddGrpcClient<ItemService.ItemServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5001"); // Server URL (same as above)
});
builder.Services.AddDistributedMemoryCache(); // Add in-memory cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
});
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
