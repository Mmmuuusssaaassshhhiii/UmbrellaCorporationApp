using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class ReportViewerForm : Form
{
    private Panel sheet = null!;
    private Button closeBtn = null!;

    public ReportViewerForm(LabReport report)
    {
        FormBorderStyle = FormBorderStyle.None;

        WindowState = FormWindowState.Maximized;

        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.Black;

        Opacity = 0.94;

        DoubleBuffered = true;

        InitializeCloseButton();

        InitializeSheet();

        InitializeHeader(report);

        InitializeClassification(report);

        InitializeTitle(report);

        InitializeMeta(report);

        InitializeContent(report);

        Resize += (s, e) =>
        {
            CenterSheet();

            closeBtn.Location = new Point(
                ClientSize.Width - closeBtn.Width - 25,
                25);
        };

        CenterSheet();
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

    private void InitializeSheet()
    {
        sheet = new Panel
        {
            Size = new Size(850, 1100),

            BackColor = Color.White
        };

        sheet.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.LightGray, 2);

            e.Graphics.DrawRectangle(
                pen,
                0,
                0,
                sheet.Width - 1,
                sheet.Height - 1);
        };

        Controls.Add(sheet);

        sheet.SendToBack();
    }

    private void InitializeHeader(LabReport report)
    {
        var logo = new PictureBox
        {
            Size = new Size(120, 120),

            Location = new Point(40, 30),

            SizeMode = PictureBoxSizeMode.Zoom,

            Image = LoadLogo(),

            BackColor = Color.Transparent
        };

        sheet.Controls.Add(logo);

        var company = new Label
        {
            Text = "UMBRELLA CORPORATION",

            Font = new Font("Exo 2", 24, FontStyle.Bold),

            ForeColor = Color.Black,

            AutoSize = true,

            Location = new Point(180, 50)
        };

        sheet.Controls.Add(company);

        var facility = new Label
        {
            Text = "RACCOON CITY RESEARCH FACILITY",

            Font = new Font("Exo 2", 12),

            ForeColor = Color.DimGray,

            AutoSize = true,

            Location = new Point(182, 95)
        };

        sheet.Controls.Add(facility);
    }

    private void InitializeClassification(LabReport report)
    {
        var classification = CreateClassification(report.ConfidentialityLevel);

        classification.Location = new Point(600, 50);

        sheet.Controls.Add(classification);
    }

    private void InitializeTitle(LabReport report)
    {
        var title = new Label
        {
            Text = report.Title,

            Font = new Font("Exo 2", 22, FontStyle.Bold),

            ForeColor = Color.Black,

            AutoSize = false,

            Width = 700,

            Height = 50,

            TextAlign = ContentAlignment.MiddleCenter,

            Location = new Point(75, 180)
        };

        sheet.Controls.Add(title);
    }

    private void InitializeMeta(LabReport report)
    {
        var meta = new Label
        {
            Text =
                $"AUTHOR: {report.Author?.FullName ?? "UNKNOWN"}\n" +
                $"DATE: {report.CreatedAt:dd.MM.yyyy HH:mm}",

            Font = new Font("Consolas", 11),

            ForeColor = Color.Black,

            AutoSize = true,

            Location = new Point(80, 260)
        };

        sheet.Controls.Add(meta);

        var line = new Panel
        {
            Height = 2,

            Width = 700,

            BackColor = Color.Black,

            Location = new Point(75, 340)
        };

        sheet.Controls.Add(line);
    }

    private void InitializeContent(LabReport report)
    {
        var content = new RichTextBox
        {
            BorderStyle = BorderStyle.None,

            BackColor = Color.White,

            ForeColor = Color.Black,

            ReadOnly = true,

            Font = new Font("Exo 2", 14),

            Location = new Point(75, 370),

            Width = 700,

            Height = 620,

            Text = report.Content,

            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        sheet.Controls.Add(content);
    }

    private void CenterSheet()
    {
        sheet.Left = (ClientSize.Width - sheet.Width) / 2;

        sheet.Top = (ClientSize.Height - sheet.Height) / 2;
    }

    private Control CreateClassification(ConfidentialityLevel level)
    {
        string text;

        Color color;

        Icon icon;

        switch (level)
        {
            case ConfidentialityLevel.Public:

                text = "PUBLIC";

                color = Color.FromArgb(70, 120, 70);

                break;

            case ConfidentialityLevel.Internal:

                text = "INTERNAL";

                color = Color.FromArgb(70, 120, 170);

                break;

            case ConfidentialityLevel.Confidential:

                text = "CONFIDENTIAL";

                color = Color.DarkOrange;

                break;

            default:

                text = "TOP SECRET";

                color = Color.DarkRed;

                break;
        }

        var panel = new Panel
        {
            Size = new Size(200, 60),

            BackColor = Color.Transparent
        };

        var pic = new PictureBox
        {
            Size = new Size(32, 32),

            Location = new Point(0, 12),

            SizeMode = PictureBoxSizeMode.StretchImage
        };

        var lbl = new Label
        {
            Text = text,

            ForeColor = color,

            Font = new Font("Arial", 14, FontStyle.Bold),

            AutoSize = true,

            Location = new Point(45, 18)
        };

        panel.Controls.Add(pic);

        panel.Controls.Add(lbl);

        return panel;
    }

    private Image? LoadLogo()
    {
        string path = "Umbrella.png";

        return System.IO.File.Exists(path)
            ? Image.FromFile(path)
            : null;
    }
}