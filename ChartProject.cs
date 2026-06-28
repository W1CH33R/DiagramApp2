namespace DiagramApp;

public class ChartDataPoint
{
    public string Label { get; set; } = "";
    public double Value { get; set; }
}

public enum ChartType
{
    Bar,
    Line
}

public class ChartProject
{
    public ChartType Type { get; set; } = ChartType.Bar;
    public List<ChartDataPoint> Points { get; set; } = new();
}
