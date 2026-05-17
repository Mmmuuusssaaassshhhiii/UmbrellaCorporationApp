using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class ReportEditorForm : Form
{
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr hWnd,
        int Msg,
        int wParam,
        int lParam);
    
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private readonly LabReport? _report;

    private TextBox titleBox = null!;

    private RichTextBox contentBox = null!;

    private ComboBox levelBox = null!;

    public ReportEditorForm(
        UmbrellaDbContext context,
        Employee currentUser,
        LabReport? report = null)
    {
        _context = context;

        _currentUser = currentUser;

        _report = report;

        InitializeUI();

        if (_report != null)
        {
            LoadReport();
        }
    }

    private void InitializeUI()
{
    Size = new Size(900, 760);

    StartPosition = FormStartPosition.CenterScreen;

    FormBorderStyle = FormBorderStyle.None;

    BackColor = Color.FromArgb(20, 0, 0);

    Padding = new Padding(1);

    MaximizeBox = false;

    // ================= TOP BAR =================

    var topBar = new Panel
    {
        Dock = DockStyle.Top,
        Height = 42,
        BackColor = Color.FromArgb(55, 0, 0)
    };

    Controls.Add(topBar);

    topBar.MouseDown += (s, e) =>
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();

            SendMessage(
                Handle,
                0xA1,
                0x2,
                0);
        }
    };

    // ================= CLOSE BUTTON =================

    var closeBtn = new Button
    {
        Text = "X",
        Dock = DockStyle.Right,
        Width = 55,
        FlatStyle = FlatStyle.Flat,
        BackColor = Color.FromArgb(55, 0, 0),
        ForeColor = Color.White,
        Font = new Font("Exo 2", 11, FontStyle.Bold),
        Cursor = Cursors.Hand
    };

    closeBtn.FlatAppearance.BorderSize = 0;

    closeBtn.MouseEnter += (s, e) =>
    {
        closeBtn.BackColor = Color.DarkRed;
    };

    closeBtn.MouseLeave += (s, e) =>
    {
        closeBtn.BackColor = Color.FromArgb(55, 0, 0);
    };

    closeBtn.Click += (s, e) =>
    {
        Close();
    };

    topBar.Controls.Add(closeBtn);

    // ================= TITLE =================

    var title = new Label
    {
        Text = _report == null
            ? "НОВЫЙ ОТЧЁТ"
            : "РЕДАКТИРОВАНИЕ ОТЧЁТА",

        Font = new Font(
            "Exo 2",
            22,
            FontStyle.Bold),

        ForeColor = Color.White,

        AutoSize = true,

        Location = new Point(30, 70)
    };

    Controls.Add(title);

    // ================= REPORT TITLE =================

    var reportTitleLabel = CreateLabel(
        "НАЗВАНИЕ ОТЧЁТА",
        30,
        140);

    Controls.Add(reportTitleLabel);

    titleBox = CreateTextBox(
        30,
        170);

    Controls.Add(titleBox);

    // ================= CONFIDENTIAL LEVEL =================

    var levelLabel = CreateLabel(
        "УРОВЕНЬ СЕКРЕТНОСТИ",
        30,
        240);

    Controls.Add(levelLabel);

    levelBox = CreateComboBox(
        30,
        270);

    levelBox.Items.AddRange(
        Enum.GetNames(
            typeof(ConfidentialityLevel)));

    levelBox.SelectedIndex = 0;

    Controls.Add(levelBox);

    // ================= CONTENT =================

    var contentLabel = CreateLabel(
        "СОДЕРЖИМОЕ ОТЧЁТА",
        30,
        340);

    Controls.Add(contentLabel);

    contentBox = new RichTextBox
    {
        Location = new Point(30, 370),

        Size = new Size(820, 250),

        Font = new Font("Exo 2", 12),

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle = BorderStyle.FixedSingle
    };

    Controls.Add(contentBox);

    // ================= SAVE BUTTON =================

    InitializeSaveButton();
}

private Label CreateLabel(
    string text,
    int x,
    int y)
{
    return new Label
    {
        Text = text,

        Location = new Point(x, y),

        AutoSize = true,

        ForeColor = Color.Gainsboro,

        Font = new Font(
            "Exo 2",
            10,
            FontStyle.Bold)
    };
}

private TextBox CreateTextBox(
    int x,
    int y)
{
    return new TextBox
    {
        Location = new Point(x, y),

        Width = 820,

        Height = 40,

        Font = new Font("Exo 2", 13),

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle = BorderStyle.FixedSingle
    };
}

private ComboBox CreateComboBox(
    int x,
    int y)
{
    return new ComboBox
    {
        Location = new Point(x, y),

        Width = 820,

        Height = 40,

        Font = new Font("Exo 2", 13),

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        DropDownStyle = ComboBoxStyle.DropDownList
    };
}

private void InitializeSaveButton()
{
    var saveBtn = new Button
    {
        Text = "СОХРАНИТЬ",

        Width = 220,

        Height = 45,

        Location = new Point(630, 660),

        FlatStyle = FlatStyle.Flat,

        BackColor = Color.FromArgb(120, 0, 0),

        ForeColor = Color.White,

        Font = new Font(
            "Exo 2",
            10,
            FontStyle.Bold),

        Cursor = Cursors.Hand
    };

    saveBtn.FlatAppearance.BorderSize = 0;

    saveBtn.MouseEnter += (s, e) =>
    {
        saveBtn.BackColor =
            Color.FromArgb(180, 0, 0);
    };

    saveBtn.MouseLeave += (s, e) =>
    {
        saveBtn.BackColor =
            Color.FromArgb(120, 0, 0);
    };

    saveBtn.Click += SaveReport;

    Controls.Add(saveBtn);
}

    private void LoadReport()
    {
        titleBox.Text = _report!.Title;

        contentBox.Text = _report.Content;

        levelBox.SelectedItem =
            _report.ConfidentialityLevel.ToString();
    }

    private void SaveReport(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleBox.Text) ||
            string.IsNullOrWhiteSpace(contentBox.Text))
        {
            MessageBox.Show("Заполни поля");

            return;
        }

        if (_report == null)
        {
            var report = new LabReport
            {
                AuthorId = _currentUser.Id,

                Title = titleBox.Text,

                Content = contentBox.Text,

                CreatedAt = DateTime.Now,

                ConfidentialityLevel =
                    Enum.Parse<ConfidentialityLevel>(
                        levelBox.SelectedItem!.ToString()!)
            };

            _context.LabReports.Add(report);
        }
        else
        {
            if (_report.AuthorId != _currentUser.Id)
            {
                MessageBox.Show(
                    "Можно редактировать только свои отчеты");

                return;
            }

            _report.Title = titleBox.Text;

            _report.Content = contentBox.Text;

            _report.ConfidentialityLevel =
                Enum.Parse<ConfidentialityLevel>(
                    levelBox.SelectedItem!.ToString()!);
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;

        Close();
    }
}