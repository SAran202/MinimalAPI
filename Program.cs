using MinimalAPI.Model;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
string dataPath = @"./data.json";

app.MapGet("/", () => "Minimal API");

app.MapGet("/get/{id:Guid?}", async (http) =>
{
	try
    {
        string txtData = File.ReadAllText(dataPath);

        http.Request.RouteValues.TryGetValue("id", out var id);

        if (!string.IsNullOrEmpty(txtData) && txtData != "{}")
            await http.Response.WriteAsJsonAsync(
                JsonSerializer.Deserialize<List<DataRecord>>(txtData).Where(r => id == null || r.Id == new Guid(id.ToString())).ToList()
                ?? new List<DataRecord>());
    }
	catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        http.Response.StatusCode = 500;
    }

    return;
});

app.MapPost("/insert", (DataRecord dataRecord) =>
{
    try
    {
        List<DataRecord> dataRecords = new ();

        string txtData = File.ReadAllText(dataPath);

        try
        {
            if (!string.IsNullOrEmpty(txtData) && txtData != "{}")
                dataRecords = JsonSerializer.Deserialize<List<DataRecord>>(txtData).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        dataRecords.Add(dataRecord);

        File.WriteAllText(dataPath, JsonSerializer.Serialize(dataRecords));

        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return Results.Problem();
    }
});

app.Run();
