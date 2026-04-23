using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// HABILITAR CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// ACTIVAR CORS
app.UseCors("AllowAll");

// ENDPOINT: /api/caseteros
app.MapGet("/api/caseteros", () =>
{
    var json = File.ReadAllText("data.json");

    // 🔥 IMPORTANTE: Permite leer propiedades en minúsculas del JSON
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    var data = JsonSerializer.Deserialize<List<Casetero>>(json, options);
    return Results.Ok(data);
});

// ENDPOINT: /api/foda/{nombre}
app.MapGet("/api/foda/{nombre}", (string nombre) =>
{
    var foda = FodaGenerator.Generate(nombre);
    return Results.Ok(foda);
});

// 🔥 CAMBIAMOS EL PUERTO PARA EVITAR EL BLOQUEO DEL 5000
app.Urls.Add("http://localhost:5158");

app.Run();

// ----------------------------
// MODELO CASETERO
// ----------------------------

public class Casetero
{
    public string Nombre { get; set; }
    public double Merma { get; set; }
    public double Limpieza { get; set; }
    public double Calidad { get; set; }
    public double Resistencia { get; set; }
    public double Tecnica { get; set; }
    public int Cajas { get; set; }
    public string Inicio { get; set; }
    public bool Baja { get; set; }
    public string Motivo { get; set; }
}

// ----------------------------
// GENERADOR DE FODA
// ----------------------------

public static class FodaGenerator
{
    public static object Generate(string nombre)
    {
        var foda = new Dictionary<string, object>
        {
            ["Miguel Gustavo Jimenez Espinoza"] = new { F="Buena calidad", O="Mejorar limpieza", D="Mermas altas", A="Estancamiento" },
            ["Leydi Lizeth Parra Leyva"] = new { F="Alta calidad", O="Mejorar resistencia", D="Carga pesada", A="Fatiga" },
            ["Jesús Abel Cervantes Medina"] = new { F="Excelente calidad", O="Mejorar rendimiento", D="Ritmo bajo", A="Productividad" },
            ["Agustín Armando Ayala Vasquez"] = new { F="Buena actitud", O="Inventarios", D="Producción baja", A="No alcanzar nivel" },
            ["Aurelia Teresa Ayala Vasquez"] = new { F="Buena calidad", O="Resistencia", D="4 horas", A="No sostener jornada" },
            ["Adolfa Balvadea Vazquez"] = new { F="Calidad aceptable", O="Técnica", D="Mermas altas", A="No completar capacitación" },
            ["Blanca Yaneli Vega Gutierrez"] = new { F="Inventarios altos", O="Orden", D="Falta material", A="Factores externos" },
            ["Felipe de Jesús Castillo Escalante"] = new { F="Condición física", O="Rendimiento", D="Apoyo en otras áreas", A="Desvío de funciones" },
            ["Michel Adriana Castro"] = new { F="Buena técnica", O="Constancia", D="Inventarios inconsistentes", A="No consolidar" },
            ["José Ángel González García"] = new { F="Mejora progresiva", O="Resistencia", D="Resistencia baja", A="Estancamiento" },
            ["Santos Heriberto Cornelio Tapia"] = new { F="Técnica aceptable", O="Limpieza", D="Inventarios bajos", A="Curva lenta" },
            ["María de los Ángeles Castillo Contreras"] = new { F="Buena calidad", O="Desarrollo", D="No completó", A="Salida" },
            ["Placencia Hernández José Miguel"] = new { F="Calidad inicial", O="Asistencia", D="Transporte", A="Inestabilidad" },
            ["Santiago Dessens Luis Enrique"] = new { F="Disponibilidad", O="Desarrollo", D="Duró 2 días", A="Abandono" },
            ["Cesar Braulio Pereida Monroy"] = new { F="Buena resistencia", O="Disciplina", D="Faltas", A="Inestabilidad" }
        };

        return foda.ContainsKey(nombre)
            ? foda[nombre]
            : new { F="N/A", O="N/A", D="N/A", A="N/A" };
    }
}

