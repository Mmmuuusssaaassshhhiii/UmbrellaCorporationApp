using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;

namespace UmbrellaCorporationApp.UI;

public class ReportsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    public ReportsControl(UmbrellaDbContext context)
    {
        _context = context;

        InitializeUI();
    }

    private void InitializeUI()
    {
        Dock = DockStyle.Fill;

        BackColor = Color.FromArgb(30, 0, 0);

        var title = new Label
        {
            Text = "ЛАБОРАТОРНЫЕ ОТЧЁТЫ",
            Dock = DockStyle.Top,
            Height = 60,
            Font = new Font("Exo 2", 20),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20, 0, 0, 0)
        };

        Controls.Add(title);

        var container = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(30, 0, 0)
        };

        Controls.Add(container);

        var reports = _context.LabReports
            .Include(x => x.Author)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        foreach (var report in reports)
        {
            container.Controls.Add(new ReportCard(report));
        }
    }
}