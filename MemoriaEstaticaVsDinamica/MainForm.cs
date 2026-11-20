using System.Windows.Forms;

namespace MemoriaEstaticaVsDinamica;

public class MainForm : Form
{
    private readonly VectorService _vectorService = new();
    private readonly BenchmarkService _benchmarkService = new();

    private NumericUpDown _nudInitialSize = null!;
    private NumericUpDown _nudNewSize = null!;
    private NumericUpDown _nudBenchElements = null!;
    private NumericUpDown _nudStaticStep = null!;

    private Label _lblStaticInfo = null!;
    private Label _lblDynamicInfo = null!;
    private TextBox _txtStaticPreview = null!;
    private TextBox _txtDynamicPreview = null!;
    private DataGridView _dgvBenchmark = null!;
    private TextBox _txtConclusion = null!;

    public MainForm()
    {
        Text = "Memoria Estatica vs. Memoria Dinamica";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(860, 640);
        Size = new Size(920, 680);

        InitializeLayout();
        RefreshVectorView();
    }

    private void InitializeLayout()
    {
        var rootLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10)
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45f));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55f));

        rootLayout.Controls.Add(BuildControlsGroup(), 0, 0);
        rootLayout.Controls.Add(BuildPreviewGroup(), 0, 1);
        rootLayout.Controls.Add(BuildBenchmarkGroup(), 0, 2);

        Controls.Add(rootLayout);
    }

    private GroupBox BuildControlsGroup()
    {
        var group = new GroupBox
        {
            Text = "Generacion y redimensionamiento en tiempo de ejecucion",
            Dock = DockStyle.Fill,
            AutoSize = true,
            Padding = new Padding(8)
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true
        };

        _nudInitialSize = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 1000000,
            Value = 10,
            Width = 90,
            Margin = new Padding(4, 6, 8, 4)
        };

        var btnGenerate = new Button
        {
            Text = "Generar vectores",
            AutoSize = true,
            Margin = new Padding(4)
        };
        btnGenerate.Click += OnGenerateClick;

        _nudNewSize = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 1000000,
            Value = 20,
            Width = 90,
            Margin = new Padding(4, 6, 8, 4)
        };

        var btnResizeStatic = new Button
        {
            Text = "Redimensionar estatico (int[])",
            AutoSize = true,
            Margin = new Padding(4)
        };
        btnResizeStatic.Click += OnResizeStaticClick;

        var btnResizeDynamic = new Button
        {
            Text = "Redimensionar dinamico (List<int>)",
            AutoSize = true,
            Margin = new Padding(4)
        };
        btnResizeDynamic.Click += OnResizeDynamicClick;

        var btnClear = new Button
        {
            Text = "Limpiar",
            AutoSize = true,
            Margin = new Padding(4)
        };
        btnClear.Click += OnClearClick;

        flow.Controls.Add(CreateLabel("Tamano inicial:"));
        flow.Controls.Add(_nudInitialSize);
        flow.Controls.Add(btnGenerate);
        flow.Controls.Add(CreateLabel("Nuevo tamano:"));
        flow.Controls.Add(_nudNewSize);
        flow.Controls.Add(btnResizeStatic);
        flow.Controls.Add(btnResizeDynamic);
        flow.Controls.Add(btnClear);

        group.Controls.Add(flow);
        return group;
    }

    private GroupBox BuildPreviewGroup()
    {
        var group = new GroupBox
        {
            Text = "Estado actual de los vectores (vista previa de hasta 50 elementos)",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        _lblStaticInfo = new Label
        {
            AutoSize = true,
            Margin = new Padding(2, 2, 2, 4)
        };

        _lblDynamicInfo = new Label
        {
            AutoSize = true,
            Margin = new Padding(2, 2, 2, 4)
        };

        _txtStaticPreview = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill
        };

        _txtDynamicPreview = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill
        };

        table.Controls.Add(_lblStaticInfo, 0, 0);
        table.Controls.Add(_lblDynamicInfo, 1, 0);
        table.Controls.Add(_txtStaticPreview, 0, 1);
        table.Controls.Add(_txtDynamicPreview, 1, 1);

        group.Controls.Add(table);
        return group;
    }

    private GroupBox BuildBenchmarkGroup()
    {
        var group = new GroupBox
        {
            Text = "Comparativa de desempeno y gestion de memoria",
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60f));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40f));

        var bar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = true
        };

        _nudBenchElements = new NumericUpDown
        {
            Minimum = 100,
            Maximum = 500000,
            Increment = 1000,
            Value = 50000,
            Width = 100,
            Margin = new Padding(4, 6, 8, 4)
        };

        _nudStaticStep = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 10000,
            Increment = 10,
            Value = 100,
            Width = 80,
            Margin = new Padding(4, 6, 8, 4)
        };

        var btnRunBenchmark = new Button
        {
            Text = "Ejecutar comparativa",
            AutoSize = true,
            Margin = new Padding(4)
        };
        btnRunBenchmark.Click += OnRunBenchmarkClick;

        bar.Controls.Add(CreateLabel("Elementos a insertar:"));
        bar.Controls.Add(_nudBenchElements);
        bar.Controls.Add(CreateLabel("Paso redimension int[]:"));
        bar.Controls.Add(_nudStaticStep);
        bar.Controls.Add(btnRunBenchmark);

        _dgvBenchmark = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        _dgvBenchmark.Columns.Add("Structure", "Estructura");
        _dgvBenchmark.Columns.Add("Operations", "Operaciones");
        _dgvBenchmark.Columns.Add("FinalSize", "Tamano final");
        _dgvBenchmark.Columns.Add("FinalCapacity", "Capacidad final");
        _dgvBenchmark.Columns.Add("ElapsedMs", "Tiempo (ms)");
        _dgvBenchmark.Columns.Add("AllocatedBytes", "Memoria asignada (bytes)");
        _dgvBenchmark.Columns.Add("Gen0", "GC Gen0");
        _dgvBenchmark.Columns.Add("Reallocs", "Reasignaciones");

        _txtConclusion = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill,
            Text = "Ejecute la comparativa para observar las diferencias de tiempo, reasignaciones y memoria asignada entre int[] y List<int>."
        };

        layout.Controls.Add(bar, 0, 0);
        layout.Controls.Add(_dgvBenchmark, 0, 1);
        layout.Controls.Add(_txtConclusion, 0, 2);

        group.Controls.Add(layout);
        return group;
    }

    private static Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Margin = new Padding(4, 9, 2, 4)
        };
    }

    private void OnGenerateClick(object? sender, EventArgs e)
    {
        int size = (int)_nudInitialSize.Value;
        _vectorService.Generate(size);
        RefreshVectorView();
    }

    private void OnResizeStaticClick(object? sender, EventArgs e)
    {
        int newSize = (int)_nudNewSize.Value;
        _vectorService.ResizeStatic(newSize);
        RefreshVectorView();
    }

    private void OnResizeDynamicClick(object? sender, EventArgs e)
    {
        int newSize = (int)_nudNewSize.Value;
        _vectorService.ResizeDynamic(newSize);
        RefreshVectorView();
    }

    private void OnClearClick(object? sender, EventArgs e)
    {
        _vectorService.Clear();
        RefreshVectorView();
    }

    private void OnRunBenchmarkClick(object? sender, EventArgs e)
    {
        int totalElements = (int)_nudBenchElements.Value;
        int staticStep = (int)_nudStaticStep.Value;

        IReadOnlyList<BenchmarkResult> results = _benchmarkService.RunGrowthComparison(totalElements, staticStep);

        _dgvBenchmark.Rows.Clear();
        foreach (BenchmarkResult r in results)
        {
            _dgvBenchmark.Rows.Add(
                r.StructureName,
                r.OperationsCount.ToString("N0"),
                r.FinalSize.ToString("N0"),
                r.FinalCapacity.ToString("N0"),
                r.ElapsedMilliseconds.ToString("F3"),
                r.AllocatedBytes.ToString("N0"),
                r.Gen0Collections.ToString("N0"),
                r.Reallocations.ToString("N0"));
        }

        if (results.Count == 2)
        {
            BenchmarkResult staticRes = results[0];
            BenchmarkResult dynamicRes = results[1];
            _txtConclusion.Text =
                $"Resultado para {totalElements:N0} elementos:{Environment.NewLine}" +
                $"- Arreglo estatico (int[], paso {staticStep}): {staticRes.ElapsedMilliseconds:F3} ms, " +
                $"{staticRes.Reallocations:N0} reasignaciones, {staticRes.AllocatedBytes:N0} bytes asignados.{Environment.NewLine}" +
                $"- Lista dinamica (List<int>): {dynamicRes.ElapsedMilliseconds:F3} ms, " +
                $"{dynamicRes.Reallocations:N0} reasignaciones, {dynamicRes.AllocatedBytes:N0} bytes asignados.{Environment.NewLine}" +
                "Al redimensionar un arreglo estatico en incrementos fijos se crean y copian bloques contiguos repetidamente, " +
                "mientras que List<T> duplica su capacidad de forma amortizada reduciendo copias y presion sobre el recolector de basura.";
        }
    }

    private void RefreshVectorView()
    {
        VectorState state = _vectorService.GetState();

        _lblStaticInfo.Text = $"Estatico (int[]): Length = {state.StaticLength:N0}";
        _lblDynamicInfo.Text = $"Dinamico (List<int>): Count = {state.DynamicCount:N0} | Capacity = {state.DynamicCapacity:N0}";

        _txtStaticPreview.Text = FormatPreview(state.StaticPreview, state.StaticLength);
        _txtDynamicPreview.Text = FormatPreview(state.DynamicPreview, state.DynamicCount);
    }

    private static string FormatPreview(int[] items, int totalCount)
    {
        if (totalCount == 0)
        {
            return "(vacio)";
        }

        string joined = string.Join(", ", items);
        if (totalCount > items.Length)
        {
            return $"{joined}, ... ({totalCount:N0} elementos en total)";
        }

        return joined;
    }
}
