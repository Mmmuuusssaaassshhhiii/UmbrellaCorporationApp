using UmbrellaCorp.Data;

namespace UmbrellaCorporationApp;

public partial class MainScreen : Form
{
    private readonly UmbrellaDbContext _context;

    public MainScreen(UmbrellaDbContext context)
    {
        InitializeComponent();
        _context = context;

        this.Load += MainScreen_Load;
    }

    private void MainScreen_Load(object sender, EventArgs e)
    {
        var count = _context.Employees.Count();
        MessageBox.Show($"Employees in system: {count}");
    }
}