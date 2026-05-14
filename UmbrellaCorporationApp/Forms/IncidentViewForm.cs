using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class IncidentViewerForm : Form
{
    private Panel sheet = null!;
    private Button closeBtn = null!;

    public IncidentViewerForm(IncidentLog log)
    {
        FormBorderStyle = FormBorderStyle.None;

        WindowState = FormWindowState.Maximized;

        BackColor = Color.Black;

        Opacity = 0.96;

        InitializeSheet();

        InitializeHeader();

        InitializeLog(log);

        InitializeCloseButton();

        Resize += (s, e) => CenterSheet();

        CenterSheet();
    }

    private void InitializeSheet()
    {
        sheet = new Panel
        {
            Size = new Size(900, 900),

            BackColor = Color.White
        };

        Controls.Add(sheet);
    }

    private void InitializeHeader()
    {
        var title = new Label
        {
            Text = "INCIDENT LOG",

            Font = new Font("Exo 2", 28, FontStyle.Bold),

            ForeColor = Color.Black,

            AutoSize = true,

            Location = new Point(60, 50)
        };

        sheet.Controls.Add(title);
    }

    private void InitializeLog(IncidentLog log)
    {
        int top = 160;

        AddField("ТИП", log.Type, top);
        top += 70;

        AddField("ЛОКАЦИЯ", log.Location, top);
        top += 70;

        AddField("УРОВЕНЬ", log.Severity.ToString(), top);
        top += 70;

        AddField("ДАТА", log.OccurredAt.ToString("dd.MM.yyyy HH:mm"), top);
        top += 70;

        AddField("СТАТУС",
            log.IsResolved ? "УСТРАНЕН" : "АКТИВЕН",
            top);

        top += 100;

        var descTitle = new Label
        {
            Text = "ОПИСАНИЕ",

            Font = new Font("Exo 2", 16, FontStyle.Bold),

            ForeColor = Color.Black,

            Location = new Point(70, top),

            AutoSize = true
        };

        sheet.Controls.Add(descTitle);

        var desc = new RichTextBox
        {
            Location = new Point(70, top + 40),

            Size = new Size(760, 400),

            ReadOnly = true,

            BorderStyle = BorderStyle.None,

            BackColor = Color.White,

            Font = new Font("Exo 2", 13),

            Text = log.Description
        };

        sheet.Controls.Add(desc);
    }

    private void AddField(
        string label,
        string value,
        int top)
    {
        var lbl = new Label
        {
            Text = label,

            Font = new Font("Exo 2", 14, FontStyle.Bold),

            ForeColor = Color.DarkRed,

            Location = new Point(70, top),

            AutoSize = true
        };

        var val = new Label
        {
            Text = value,

            Font = new Font("Exo 2", 14),

            ForeColor = Color.Black,

            Location = new Point(300, top),

            AutoSize = true
        };

        sheet.Controls.Add(lbl);

        sheet.Controls.Add(val);
    }

    private void InitializeCloseButton()
    {
        closeBtn = new Button
        {
            Text = "X",

            Width = 60,
            Height = 60,

            FlatStyle = FlatStyle.Flat,

            Font = new Font("Exo 2", 18, FontStyle.Bold),

            ForeColor = Color.White,

            Cursor = Cursors.Hand,

            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };

        closeBtn.FlatAppearance.BorderSize = 0;

        closeBtn.Location = new Point(
            ClientSize.Width - closeBtn.Width - 25,
            25);

        closeBtn.MouseEnter += (s, e) =>
        {
            closeBtn.ForeColor = Color.Gray;
        };

        closeBtn.MouseLeave += (s, e) =>
        {
            closeBtn.ForeColor = Color.White;
        };

        closeBtn.Click += (s, e) => Close();

        Controls.Add(closeBtn);

        closeBtn.BringToFront();
    }

    private void CenterSheet()
    {
        sheet.Left = (ClientSize.Width - sheet.Width) / 2;

        sheet.Top = (ClientSize.Height - sheet.Height) / 2;
    }
}