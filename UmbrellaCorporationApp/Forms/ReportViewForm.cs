using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ReportViewerForm : Form
{
    public ReportViewerForm(LabReport report)
    {
        Text = report.Title;

        WindowState = FormWindowState.Maximized;

        BackColor = Color.Black;

        var title = new Label
        {
            Text = report.Title,
            Dock = DockStyle.Top,
            Height = 70,
            Font = new Font("Consolas", 22, FontStyle.Bold),
            ForeColor = Color.Red,
            Padding = new Padding(20, 15, 0, 0)
        };

        var info = new Label
        {
            Text =
                $"AUTHOR: {report.Author?.FullName}    " +
                $"DATE: {report.CreatedAt:dd.MM.yyyy HH:mm}    " +
                $"CLEARANCE: {report.ConfidentialityLevel}",

            Dock = DockStyle.Top,
            Height = 40,
            Font = new Font("Consolas", 10),
            ForeColor = Color.Gray,
            Padding = new Padding(20, 0, 0, 0)
        };

        var content = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.Black,
            ForeColor = Color.White,
            Font = new Font("Consolas", 14),
            Text = report.Content
        };

        Controls.Add(content);
        Controls.Add(info);
        Controls.Add(title);
    }
}