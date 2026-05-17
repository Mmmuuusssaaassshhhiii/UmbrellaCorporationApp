using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class IncidentEditorForm : Form
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

    private readonly IncidentLog? _log;

    private TextBox typeBox = null!;

    private TextBox locationBox = null!;

    private ComboBox severityBox = null!;

    private RichTextBox descriptionBox = null!;

    private CheckBox resolvedBox = null!;

    public IncidentEditorForm(
        UmbrellaDbContext context,
        Employee currentUser,
        IncidentLog? log = null)
    {
        _context = context;

        _currentUser = currentUser;

        _log = log;

        InitializeUI();

        if (_log != null)
        {
            LoadLog();
        }
    }

    private void InitializeUI()
{
    Size = new Size(900, 760);

    StartPosition = FormStartPosition.CenterScreen;

    BackColor = Color.FromArgb(20, 0, 0);

    FormBorderStyle = FormBorderStyle.None;

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
        Text = _log == null
            ? "НОВОЕ ПРОИСШЕСТВИЕ"
            : "РЕДАКТИРОВАНИЕ ПРОИСШЕСТВИЯ",

        Font = new Font(
            "Exo 2",
            22,
            FontStyle.Bold),

        ForeColor = Color.White,

        AutoSize = true,

        Location = new Point(30, 70)
    };

    Controls.Add(title);

    // ================= TYPE =================

    Controls.Add(CreateLabel(
        "ТИП ПРОИСШЕСТВИЯ",
        30,
        140));

    typeBox = CreateTextBox(
        30,
        170);

    Controls.Add(typeBox);

    // ================= LOCATION =================

    Controls.Add(CreateLabel(
        "МЕСТО ПРОИСШЕСТВИЯ",
        30,
        240));

    locationBox = CreateTextBox(
        30,
        270);

    Controls.Add(locationBox);

    // ================= SEVERITY =================

    Controls.Add(CreateLabel(
        "ТЯЖЕСТЬ",
        30,
        340));

    severityBox = CreateComboBox(
        30,
        370);

    severityBox.Items.AddRange(
        Enum.GetNames(
            typeof(IncidentSeverity)));

    severityBox.SelectedIndex = 0;

    Controls.Add(severityBox);

    // ================= RESOLVED =================

    resolvedBox = new CheckBox
    {
        Text = "Инцидент устранен",

        Location = new Point(30, 440),

        AutoSize = true,

        ForeColor = Color.White,

        BackColor = Color.Transparent,

        Font = new Font(
            "Exo 2",
            11,
            FontStyle.Bold)
    };

    Controls.Add(resolvedBox);

    // ================= DESCRIPTION =================

    Controls.Add(CreateLabel(
        "ОПИСАНИЕ",
        30,
        490));

    descriptionBox = new RichTextBox
    {
        Location = new Point(30, 520),

        Size = new Size(820, 140),

        Font = new Font("Consolas", 12),

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle = BorderStyle.FixedSingle
    };

    Controls.Add(descriptionBox);

    // ================= SAVE BUTTON =================

    var saveBtn = new Button
    {
        Text = "СОХРАНИТЬ",

        Width = 220,

        Height = 45,

        Location = new Point(630, 680),

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

    saveBtn.Click += SaveLog;

    Controls.Add(saveBtn);
}

    private void LoadLog()
    {
        typeBox.Text = _log!.Type;

        locationBox.Text = _log.Location;

        descriptionBox.Text = _log.Description;

        resolvedBox.Checked = _log.IsResolved;

        severityBox.SelectedItem =
            _log.Severity.ToString();
    }

    private void SaveLog(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(typeBox.Text) ||
            string.IsNullOrWhiteSpace(descriptionBox.Text))
        {
            MessageBox.Show("Заполни поля");

            return;
        }

        if (_log == null)
        {
            var log = new IncidentLog
            {
                Type = typeBox.Text,

                Location = locationBox.Text,

                Description = descriptionBox.Text,

                ReportedById = _currentUser.Id,

                OccurredAt = DateTime.Now,

                IsResolved = resolvedBox.Checked,

                Severity = Enum.Parse<IncidentSeverity>(
                    severityBox.SelectedItem!.ToString()!)
            };

            _context.IncidentLogs.Add(log);
        }
        else
        {
            if (_log.ReportedById != _currentUser.Id)
            {
                MessageBox.Show(
                    "Можно редактировать только свои записи");

                return;
            }

            _log.Type = typeBox.Text;

            _log.Location = locationBox.Text;

            _log.Description = descriptionBox.Text;

            _log.IsResolved = resolvedBox.Checked;

            _log.Severity = Enum.Parse<IncidentSeverity>(
                severityBox.SelectedItem!.ToString()!);
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;

        Close();
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
}