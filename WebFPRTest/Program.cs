using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using WebFPRTest.Areas.Externo.Interface.Acreditacion;
using WebFPRTest.Areas.Externo.Interface.Equipo;
using WebFPRTest.Areas.Externo.Interface.Jugador;
using WebFPRTest.Areas.Externo.Service.Acreditacion;
using WebFPRTest.Areas.Externo.Service.Equipo;
using WebFPRTest.Areas.Externo.Service.Jugador;
using WebFPRTest.Areas.Interno.Interface.ListAcreditacion;
using WebFPRTest.Areas.Interno.Interface.ListJugadores;
using WebFPRTest.Areas.Interno.Interface.Usuario;
using WebFPRTest.Areas.Interno.Service.ListAcreditacion;
using WebFPRTest.Areas.Interno.Service.ListJugadores;
using WebFPRTest.Areas.Interno.Service.Usuario;
using WebFPRTest.Interface;
using WebFPRTest.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Para llamar a los servicios y a las interfaces
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ITiposService, TiposService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IJugadorService, JugadorService>();
builder.Services.AddScoped<IAcreditacionService, AcreditacionService>();
builder.Services.AddScoped<IListJugadoresService, ListJugadoresService>();
builder.Services.AddScoped<IListAcreditacionService, ListAcreditacionService>();


// Para conectar a la BD
builder.Services.AddScoped<SqlConnection>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("FPRConnection");
    return new SqlConnection(connectionString);
});
// **Agregar autenticación con cookies**
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login"; // Redirige al login si no está autenticado
        options.AccessDeniedPath = "/Login/AccesoDenegado"; // Página de acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Expiración de la cookie
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔹 Agregar autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

// Configurar rutas de áreas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "externo",
    pattern: "Externo/{controller=Equipo}/{action=Equipo}/{id?}");

app.MapControllerRoute(
    name: "interno",
    pattern: "Interno/{controller=Jugadores}/{action=Jugadores}/{id?}");

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
