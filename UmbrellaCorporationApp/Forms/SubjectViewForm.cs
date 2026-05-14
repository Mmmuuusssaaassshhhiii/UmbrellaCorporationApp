using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.Forms;

public class SubjectViewForm : Form
{
    private Panel sheet = null!;

    public SubjectViewForm(TestSubject subject)
    {
        FormBorderStyle = FormBorderStyle.None;

        WindowState = FormWindowState.Maximized;

        BackColor = Color.Black;

        Opacity = 0.96;

        InitializeSheet();

        InitializeHeader();

        InitializeSubject(subject);

        InitializeCloseButton();

        Resize += (s, e) => CenterSheet();

        CenterSheet();
    }

    private void InitializeSheet()
    {
        sheet = new Panel
        {
            Size = new Size(900, 950),

            BackColor = Color.White
        };

        Controls.Add(sheet);
    }

    private void InitializeHeader()
    {
        var logo = new PictureBox
        {
            Size = new Size(120, 120),

            Location = new Point(40, 30),

            SizeMode = PictureBoxSizeMode.Zoom,

            Image = LoadLogo()
        };

        sheet.Controls.Add(logo);

        var title = new Label
        {
            Text = "TEST SUBJECT FILE",

            Font = new Font("Exo 2", 28, FontStyle.Bold),

            ForeColor = Color.Black,

            AutoSize = true,

            Location = new Point(190, 55)
        };

        sheet.Controls.Add(title);
    }

    private void InitializeSubject(TestSubject subject)
    {
        int top = 180;

        AddField("КОД", subject.Code, top);
        top += 70;

        AddField(
            "ВИРУС",
            subject.Virus?.Name ?? "UNKNOWN",
            top);

        top += 70;

        AddField(
            "СТАТУС",
            subject.Status.ToString(),
            top);

        top += 70;

        AddField(
            "МЕСТОНАХОЖДЕНИЕ",
            subject.Location,
            top);

        top += 100;

        AddField(
            "ДАТА ПРИОБРЕТЕНИЯ",
            subject.AcquiredDate.ToString("dd.MM.yyyy"),
            top);
        
        top += 100;
        
        AddField("ЗАМЕТКИ", subject.Notes, top);
        top += 100;

        var notesTitle = new Label
        {
            Text = "ЗАМЕТКИ",

            Font = new Font("Exo 2", 16, FontStyle.Bold),

            ForeColor = Color.Black,

            Location = new Point(70, top),

            AutoSize = true
        };

        sheet.Controls.Add(notesTitle);

        var notes = new RichTextBox
        {
            Location = new Point(70, top + 40),

            Size = new Size(760, 400),

            ReadOnly = true,

            BorderStyle = BorderStyle.None,

            BackColor = Color.White,

            Font = new Font("Consolas", 13),

            Text = subject.Notes
        };

        sheet.Controls.Add(notes);
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

            Font = new Font("Consolas", 14),

            ForeColor = Color.Black,

            Location = new Point(250, top),

            AutoSize = true
        };

        sheet.Controls.Add(lbl);

        sheet.Controls.Add(val);
    }

    private void InitializeCloseButton()
    {
        var closeBtn = new Button
        {
            Text = "X",

            Width = 60,

            Height = 60,

            FlatStyle = FlatStyle.Flat,

            ForeColor = Color.White,

            Font = new Font("Exo 2", 18, FontStyle.Bold),

            Location = new Point(
                ClientSize.Width - 90,
                20)
        };

        closeBtn.FlatAppearance.BorderSize = 0;

        closeBtn.Click += (s, e) => Close();

        Controls.Add(closeBtn);

        closeBtn.BringToFront();
    }

    private void CenterSheet()
    {
        sheet.Left = (ClientSize.Width - sheet.Width) / 2;

        sheet.Top = (ClientSize.Height - sheet.Height) / 2;
    }

    private Image? LoadLogo()
    {
        string path = "Umbrella.png";

        return File.Exists(path)
            ? Image.FromFile(path)
            : null;
    }
}