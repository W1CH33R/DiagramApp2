using System.ComponentModel;

namespace DiagramApp;

public class MainForm : Form
{
    private readonly ProjectStorage _storage = new();
    private ChartProject _project;

    private DataGridView _grid;
    private ComboBox _typeCombo;
    private DoubleBufferedPanel _chartPanel;
    private BindingList<ChartDataPoint> _bindingList;

    public MainForm()
    {
        Text = "Prosty generator diagramów";
        Width = 1100;
        Height = 700;
        MinimumSize = new Size(700, 500);
        StartPosition = FormStartPosition.CenterScreen;

        _project = _storage.Load();

        BuildUi();
        RefreshGridFromProject();

        FormClosing += (s, e) => SaveCurrentState();
    }

    private void BuildUi()
    {
        var topPanel = new Panel { Dock = DockStyle.Top, Height = 45 };
        var lbl = new Label { Text = "Typ diagramu:", Left = 10, Top = 13, AutoSize = true };
        _typeCombo = new ComboBox
        {
            Left = 120,
            Top = 9,
            Width = 220,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _typeCombo.Items.AddRange(new object[] { "Słupkowy", "Liniowy" });
        _typeCombo.SelectedIndex = _project.Type == ChartType.Bar ? 0 : 1;
        _typeCombo.SelectedIndexChanged += (s, e) =>
        {
            _project.Type = _typeCombo.SelectedIndex == 0 ? ChartType.Bar : ChartType.Line;
            _chartPanel.Invalidate();
        };
        topPanel.Controls.Add(lbl);
        topPanel.Controls.Add(_typeCombo);

        var leftContainer = new Panel { Dock = DockStyle.Left, Width = 360 };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            AllowUserToAddRows = true,
            AllowUserToDeleteRows = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        };
        _grid.CellValueChanged += (s, e) => _chartPanel.Invalidate();
        _grid.DataError += (s, e) => e.ThrowException = false;

        var addButton = new Button { Text = "Dodaj wiersz", Dock = DockStyle.Bottom, Height = 32 };
        addButton.Click += (s, e) =>
        {
            _bindingList.Add(new ChartDataPoint { Label = "Nowa", Value = 10 });
        };

        leftContainer.Controls.Add(_grid);
        leftContainer.Controls.Add(addButton);

        _chartPanel = new DoubleBufferedPanel { Dock = DockStyle.Fill, BackColor = Color.White };
        _chartPanel.Paint += ChartPanel_Paint;

        Controls.Add(topPanel);
        Controls.Add(leftContainer);
        Controls.Add(_chartPanel);
    }

    private void RefreshGridFromProject()
    {
        _bindingList = new BindingList<ChartDataPoint>(_project.Points);
        _bindingList.ListChanged += (s, e) => _chartPanel.Invalidate();
        _grid.DataSource = _bindingList;
    }

    private void ChartPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        var area = new Rectangle(40, 20, _chartPanel.Width - 80, _chartPanel.Height - 80);
        var points = _project.Points.Where(p => !string.IsNullOrWhiteSpace(p.Label)).ToList();

        if (points.Count == 0)
        {
            g.DrawString("Dodaj dane w tabeli po lewej stronie", Font, Brushes.Gray, 20, 20);
            return;
        }

        double max = points.Max(p => p.Value);
        if (max <= 0) max = 1;

        g.DrawLine(Pens.Black, area.Left, area.Top, area.Left, area.Bottom);
        g.DrawLine(Pens.Black, area.Left, area.Bottom, area.Right, area.Bottom);

        if (_project.Type == ChartType.Bar)
        {
            DrawBarChart(g, area, points, max);
        }
        else
        {
            DrawLineChart(g, area, points, max);
        }
    }

    private void DrawBarChart(Graphics g, Rectangle area, List<ChartDataPoint> points, double max)
    {
        int n = points.Count;
        int slot = area.Width / n;
        using var brush = new SolidBrush(Color.SteelBlue);

        for (int i = 0; i < n; i++)
        {
            var p = points[i];
            int barHeight = (int)(p.Value / max * (area.Height - 10));
            int x = area.Left + i * slot + 10;
            int barWidth = Math.Max(slot - 20, 5);
            int y = area.Bottom - barHeight;

            g.FillRectangle(brush, x, y, barWidth, barHeight);
            g.DrawString(p.Value.ToString("0.##"), Font, Brushes.Black, x, y - 16);
            g.DrawString(p.Label, Font, Brushes.Black, x, area.Bottom + 4);
        }
    }

    private void DrawLineChart(Graphics g, Rectangle area, List<ChartDataPoint> points, double max)
    {
        int n = points.Count;
        int step = n > 1 ? area.Width / (n - 1) : 0;
        var pts = new List<PointF>();

        for (int i = 0; i < n; i++)
        {
            var p = points[i];
            int x = area.Left + i * step;
            int y = (int)(area.Bottom - (p.Value / max * (area.Height - 10)));
            pts.Add(new PointF(x, y));
            g.DrawString(p.Label, Font, Brushes.Black, x - 10, area.Bottom + 4);
            g.DrawString(p.Value.ToString("0.##"), Font, Brushes.Black, x - 10, y - 18);
        }

        if (pts.Count > 1)
            g.DrawLines(new Pen(Color.DarkOrange, 2), pts.ToArray());

        foreach (var pt in pts)
            g.FillEllipse(Brushes.DarkOrange, pt.X - 3, pt.Y - 3, 6, 6);
    }

    private void SaveCurrentState()
    {
        _grid.EndEdit();
        _project.Points = _bindingList.ToList();
        _storage.Save(_project);
    }
}

public class DoubleBufferedPanel : Panel
{
    public DoubleBufferedPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}
