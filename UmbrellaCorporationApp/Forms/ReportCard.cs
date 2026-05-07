using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ReportCard : Panel
{
    private readonly LabReport _report;

    public ReportCard(LabReport report)
    {
        _report = report;

        Width = 180;
        Height = 230;

        BackColor = Color.FromArgb(45, 0, 0);

        Margin = new Padding(20);

        Cursor = Cursors.Hand;

        var icon = new PictureBox
        {
            Dock = DockStyle.Top,
            Height = 120,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = SystemIcons.Application.ToBitmap(),
            BackColor = Color.Transparent
        };

        var title = new Label
        {
            Text = report.Title,
            Dock = DockStyle.Top,
            Height = 50,
            ForeColor = Color.White,
            Font = new Font("Exo 2", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var author = new Label
        {
            Text = $"Автор: {report.Author?.FullName ?? "UNKNOWN"}",
            Dock = DockStyle.Top,
            Height = 30,
            ForeColor = Color.Gray,
            Font = new Font("Exo 2", 8),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var date = new Label
        {
            Text = report.CreatedAt.ToString("dd.MM.yyyy"),
            Dock = DockStyle.Fill,
            ForeColor = Color.DarkGray,
            Font = new Font("Exo 2", 8),
            TextAlign = ContentAlignment.TopCenter
        };

        Controls.Add(date);
        Controls.Add(author);
        Controls.Add(title);
        Controls.Add(icon);

        DoubleClick += OpenReport;
        icon.DoubleClick += OpenReport;
        title.DoubleClick += OpenReport;
        author.DoubleClick += OpenReport;
        date.DoubleClick += OpenReport;

        MouseEnter += (s, e) =>
        {
            BackColor = Color.FromArgb(90, 0, 0);
        };

        MouseLeave += (s, e) =>
        {
            BackColor = Color.FromArgb(45, 0, 0);
        };
    }

    private void OpenReport(object? sender, EventArgs e)
    {
        new ReportViewerForm(_report).ShowDialog();
    }
}