using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class DevelopmentEditorForm : Form
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

    private readonly Development? _development;

    private ComboBox virusBox = null!;

    private ComboBox scientistBox = null!;

    private ComboBox statusBox = null!;

    private TextBox projectBox = null!;

    private DateTimePicker startPicker = null!;

    private DateTimePicker endPicker = null!;

    public DevelopmentEditorForm(
        UmbrellaDbContext context,
        Development? development = null)
    {
        _context = context;

        _development = development;

        InitializeUI();

        if (_development != null)
        {
            LoadDevelopment();
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
            Text = _development == null
                ? "НОВАЯ РАЗРАБОТКА"
                : "РЕДАКТИРОВАНИЕ РАЗРАБОТКИ",

            Font = new Font(
                "Exo 2",
                22,
                FontStyle.Bold),

            ForeColor = Color.White,

            AutoSize = true,

            Location = new Point(30, 70)
        };

        Controls.Add(title);

        // ================= PROJECT =================

        Controls.Add(CreateLabel(
            "НАЗВАНИЕ ПРОЕКТА",
            30,
            140));

        projectBox = CreateTextBox(
            30,
            170);

        Controls.Add(projectBox);

        // ================= VIRUS =================

        Controls.Add(CreateLabel(
            "ВИРУС",
            30,
            240));

        virusBox = CreateComboBox(
            30,
            270);

        virusBox.DataSource =
            _context.Viruses.ToList();

        virusBox.DisplayMember = "Name";

        virusBox.ValueMember = "Id";

        Controls.Add(virusBox);

        // ================= SCIENTIST =================

        Controls.Add(CreateLabel(
            "ГЛАВНЫЙ УЧЕНЫЙ",
            30,
            340));

        scientistBox = CreateComboBox(
            30,
            370);

        scientistBox.DataSource =
            _context.Employees.ToList();

        scientistBox.DisplayMember = "FullName";

        scientistBox.ValueMember = "Id";

        Controls.Add(scientistBox);

        // ================= STATUS =================

        Controls.Add(CreateLabel(
            "СТАТУС",
            30,
            440));

        statusBox = CreateComboBox(
            30,
            470);

        statusBox.Items.AddRange(
            Enum.GetNames(
                typeof(DevelopmentStatus)));

        Controls.Add(statusBox);

        // ================= START DATE =================

        Controls.Add(CreateLabel(
            "ДАТА НАЧАЛА",
            30,
            540));

        startPicker = new DateTimePicker
        {
            Location = new Point(30, 570),

            Width = 350,

            Font = new Font("Exo 2", 11),

            CalendarMonthBackground =
                Color.FromArgb(40, 0, 0)
        };

        Controls.Add(startPicker);

        // ================= END DATE =================

        Controls.Add(CreateLabel(
            "ДАТА ОКОНЧАНИЯ",
            450,
            540));

        endPicker = new DateTimePicker
        {
            Location = new Point(450, 570),

            Width = 350,

            Font = new Font("Exo 2", 11),

            CalendarMonthBackground =
                Color.FromArgb(40, 0, 0)
        };

        Controls.Add(endPicker);

        // ================= SAVE BUTTON =================

        var saveBtn = new Button
        {
            Text = _development == null
                ? "СОХРАНИТЬ"
                : "ОБНОВИТЬ",

            Width = 220,

            Height = 45,

            Location = new Point(630, 670),

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

        saveBtn.Click += SaveDevelopment;

        Controls.Add(saveBtn);
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

    private void LoadDevelopment()
    {
        projectBox.Text =
            _development!.ProjectName;

        virusBox.SelectedValue =
            _development.VirusId;

        scientistBox.SelectedValue =
            _development.LeadScientistId;

        statusBox.SelectedItem =
            _development.Status.ToString();

        startPicker.Value =
            _development.StartDate;

        if (_development.EndDate.HasValue)
        {
            endPicker.Value =
                _development.EndDate.Value;
        }
    }

    private void SaveDevelopment(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                projectBox.Text))
        {
            MessageBox.Show(
                "Введите название проекта");

            return;
        }

        if (_development == null)
        {
            var development = new Development
            {
                ProjectName =
                    projectBox.Text,

                VirusId =
                    (int)virusBox.SelectedValue,

                LeadScientistId =
                    (int)scientistBox.SelectedValue,

                Status =
                    Enum.Parse<DevelopmentStatus>(
                        statusBox.SelectedItem!
                            .ToString()!),

                StartDate =
                    startPicker.Value,

                EndDate =
                    endPicker.Value
            };

            _context.Developments
                .Add(development);
        }
        else
        {
            _development.ProjectName =
                projectBox.Text;

            _development.VirusId =
                (int)virusBox.SelectedValue;

            _development.LeadScientistId =
                (int)scientistBox.SelectedValue;

            _development.Status =
                Enum.Parse<DevelopmentStatus>(
                    statusBox.SelectedItem!
                        .ToString()!);

            _development.StartDate =
                startPicker.Value;

            _development.EndDate =
                endPicker.Value;
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
}