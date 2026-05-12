using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class FileViewerForm : Form
{
    private Panel sheet = null!;
    private Button closeBtn = null!;

    public FileViewerForm(ClassifiedFile file)
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.Black;
        Opacity = 0.94;
        DoubleBuffered = true;

        InitializeCloseButton();
        InitializeSheet();
        InitializeHeader(file);
        InitializeClassification(file);
        InitializeTitle(file);
        InitializeMeta(file);
        InitializeContent(file);

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

        closeBtn.MouseEnter += (s, e) => closeBtn.ForeColor = Color.Gray;
        closeBtn.MouseLeave += (s, e) => closeBtn.ForeColor = Color.White;
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

    private void InitializeHeader(ClassifiedFile file)
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

        sheet.Controls.Add(new Label
        {
            Text = "UMBRELLA CORPORATION",
            Font = new Font("Exo 2", 24, FontStyle.Bold),
            ForeColor = Color.Black,
            AutoSize = true,
            Location = new Point(180, 50)
        });

        sheet.Controls.Add(new Label
        {
            Text = "RACCOON CITY RESEARCH FACILITY",
            Font = new Font("Exo 2", 12),
            ForeColor = Color.DimGray,
            AutoSize = true,
            Location = new Point(182, 95)
        });
    }

    private void InitializeClassification(ClassifiedFile file)
    {
        var classification = CreateClassification(file.Level);
        classification.Location = new Point(600, 50);
        sheet.Controls.Add(classification);
    }

    private void InitializeTitle(ClassifiedFile file)
    {
        sheet.Controls.Add(new Label
        {
            Text = file.Title,
            Font = new Font("Exo 2", 22, FontStyle.Bold),
            ForeColor = Color.Black,
            AutoSize = false,
            Width = 700,
            Height = 50,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(75, 180)
        });
    }

    private void InitializeMeta(ClassifiedFile file)
    {
        sheet.Controls.Add(new Label
        {
            Text =
                $"AUTHOR: {file.Author?.FullName ?? "UNKNOWN"}\n" +
                $"DATE: {file.CreatedAt:dd.MM.yyyy HH:mm}",
            Font = new Font("Consolas", 11),
            ForeColor = Color.Black,
            AutoSize = true,
            Location = new Point(80, 260)
        });

        sheet.Controls.Add(new Panel
        {
            Height = 2,
            Width = 700,
            BackColor = Color.Black,
            Location = new Point(75, 340)
        });
    }

    private void InitializeContent(ClassifiedFile file)
    {
        sheet.Controls.Add(new RichTextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor = Color.White,
            ForeColor = Color.Black,
            ReadOnly = true,
            Font = new Font("Exo 2", 14),
            Location = new Point(75, 370),
            Width = 700,
            Height = 620,
            Text = file.Content,
            ScrollBars = RichTextBoxScrollBars.Vertical
        });
    }

    private void CenterSheet()
    {
        sheet.Left = (ClientSize.Width - sheet.Width) / 2;
        sheet.Top = (ClientSize.Height - sheet.Height) / 2;
    }

    private Control CreateClassification(FileLevel level)
    {
        string text;
        Color color;

        switch (level)
        {
            case FileLevel.Confidential:
                text = "CONFIDENTIAL";
                color = Color.DarkOrange;
                break;

            case FileLevel.Secret:
                text = "SECRET";
                color = Color.FromArgb(70, 120, 170);
                break;

            case FileLevel.TopSecret:
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

        var lbl = new Label
        {
            Text = text,
            ForeColor = color,
            Font = new Font("Arial", 14, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(45, 18)
        };

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