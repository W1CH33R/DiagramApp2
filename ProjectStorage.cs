using System.Text.Json;

namespace DiagramApp;

public class ProjectStorage
{
    private readonly string _filePath;

    public ProjectStorage()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "diagram_state.json");
    }

    public void Save(ChartProject project)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(project, options);
        File.WriteAllText(_filePath, json);
    }

    public ChartProject Load()
    {
        if (!File.Exists(_filePath))
            return new ChartProject();

        try
        {
            string json = File.ReadAllText(_filePath);
            var project = JsonSerializer.Deserialize<ChartProject>(json);
            return project ?? new ChartProject();
        }
        catch
        {
            return new ChartProject();
        }
    }
}
