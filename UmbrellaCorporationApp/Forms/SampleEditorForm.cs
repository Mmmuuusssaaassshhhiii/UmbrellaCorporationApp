using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class SampleEditorForm : Form
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

    private readonly Sample? _sample;

    private ComboBox virusBox = null!;
    private ComboBox scientistBox = null!;

    private TextBox storageBox = null!;

    private DateTimePicker createdPicker = null!;

    private RichTextBox notesBox = null!;

    private CheckBox destroyedBox = null!;

    public SampleEditorForm(
        UmbrellaDbContext context,
        Sample? sample = null)
    {
        _context = context;

        _sample = sample;

        InitializeUI();

        LoadComboData();

        if (_sample != null)
        {
            LoadSample();
        }
    }

    private void InitializeUI()
{
    Size = new Size(900, 800);

    StartPosition =
        FormStartPosition.CenterScreen;

    BackColor =
        Color.FromArgb(20, 0, 0);

    FormBorderStyle =
        FormBorderStyle.None;

    Padding =
        new Padding(1);

    MaximizeBox = false;

    // ================= TOP BAR =================

    var topBar = new Panel
    {
        Dock = DockStyle.Top,

        Height = 42,

        BackColor =
            Color.FromArgb(55, 0, 0)
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

        BackColor =
            Color.FromArgb(55, 0, 0),

        ForeColor = Color.White,

        Font = new Font(
            "Exo 2",
            11,
            FontStyle.Bold),

        Cursor = Cursors.Hand
    };

    closeBtn.FlatAppearance.BorderSize = 0;

    closeBtn.MouseEnter += (s, e) =>
    {
        closeBtn.BackColor = Color.DarkRed;
    };

    closeBtn.MouseLeave += (s, e) =>
    {
        closeBtn.BackColor =
            Color.FromArgb(55, 0, 0);
    };

    closeBtn.Click += (s, e) =>
    {
        Close();
    };

    topBar.Controls.Add(closeBtn);

    // ================= TITLE =================

    var title = new Label
    {
        Text = _sample == null
            ? "НОВЫЙ ОБРАЗЕЦ"
            : "РЕДАКТИРОВАНИЕ ОБРАЗЦА",

        Font = new Font(
            "Exo 2",
            22,
            FontStyle.Bold),

        ForeColor = Color.White,

        AutoSize = true,

        Location = new Point(30, 70)
    };

    Controls.Add(title);

    int left = 30;
    int width = 820;
    int y = 130;

    // ================= VIRUS =================

    Controls.Add(CreateLabel("ВИРУС", left, y));

    virusBox = CreateComboBox(left, y + 30, width);

    virusBox.DataSource = _context.Viruses.ToList();
    virusBox.DisplayMember = "Name";
    virusBox.ValueMember = "Id";

    Controls.Add(virusBox);

    // ================= SCIENTIST =================

    y += 90;

    Controls.Add(CreateLabel("ОТВЕТСТВЕННЫЙ УЧЕНЫЙ", left, y));

    scientistBox = CreateComboBox(left, y + 30, width);

    scientistBox.DataSource = _context.Employees.ToList();
    scientistBox.DisplayMember = "FullName";
    scientistBox.ValueMember = "Id";

    Controls.Add(scientistBox);

    // ================= STORAGE =================

    y += 90;

    Controls.Add(CreateLabel("МЕСТОНАХОЖДЕНИЕ", left, y));

    storageBox = CreateTextBox(left, y + 30, width);

    Controls.Add(storageBox);

    // ================= DATE =================

    y += 90;

    Controls.Add(CreateLabel("ДАТА СОЗДАНИЯ", left, y));

    createdPicker = new DateTimePicker
    {
        Location = new Point(left, y + 30),

        Width = 300,

        Font = new Font("Exo 2", 11),

        CalendarMonthBackground =
            Color.FromArgb(40, 0, 0)
    };

    Controls.Add(createdPicker);

    destroyedBox = new CheckBox
    {
        Text = "Образец уничтожен",

        Location = new Point(340, y + 32),

        ForeColor = Color.White,

        BackColor = Color.Transparent,

        Font = new Font(
            "Exo 2",
            11,
            FontStyle.Bold),

        AutoSize = true
    };

    Controls.Add(destroyedBox);

    // ================= NOTES =================

    y += 90;

    Controls.Add(CreateLabel("ЗАМЕТКИ", left, y));

    notesBox = new RichTextBox
    {
        Location = new Point(left, y + 30),

        Size = new Size(width, 160),

        BackColor =
            Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle = BorderStyle.FixedSingle,

        Font = new Font("Exo 2", 11)
    };

    Controls.Add(notesBox);

    // ================= SAVE BUTTON =================

    var saveBtn = new Button
    {
        Text = _sample == null ? "СОХРАНИТЬ" : "ОБНОВИТЬ",

        Width = 220,

        Height = 45,

        Location = new Point(660, 700),

        FlatStyle = FlatStyle.Flat,

        BackColor =
            Color.FromArgb(120, 0, 0),

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

    saveBtn.Click += SaveSample;

    Controls.Add(saveBtn);
}

private Label CreateLabel(string text, int x, int y)
{
    return new Label
    {
        Text = text,

        Location = new Point(x, y),

        AutoSize = true,

        ForeColor = Color.White,

        Font = new Font(
            "Exo 2",
            10,
            FontStyle.Bold)
    };
}

private TextBox CreateTextBox(int x, int y, int width)
{
    return new TextBox
    {
        Location = new Point(x, y),

        Width = width,

        Height = 35,

        BorderStyle = BorderStyle.FixedSingle,

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        Font = new Font("Exo 2", 11)
    };
}

private ComboBox CreateComboBox(int x, int y, int width)
{
    return new ComboBox
    {
        Location = new Point(x, y),

        Width = width,

        DropDownStyle = ComboBoxStyle.DropDownList,

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        Font = new Font("Exo 2", 11)
    };
}

    private void LoadComboData()
    {
        virusBox.DataSource =
            _context.Viruses.ToList();

        virusBox.DisplayMember = "Name";

        virusBox.ValueMember = "Id";

        scientistBox.DataSource =
            _context.Employees.ToList();

        scientistBox.DisplayMember = "FullName";

        scientistBox.ValueMember = "Id";
    }

    private void LoadSample()
    {
        virusBox.SelectedValue =
            _sample!.VirusId;

        scientistBox.SelectedValue =
            _sample.ResponsibleScientistId;

        storageBox.Text =
            _sample.StorageLocation;

        createdPicker.Value =
            _sample.CreatedAt;

        notesBox.Text =
            _sample.Notes;

        destroyedBox.Checked =
            _sample.IsDestroyed;
    }

    private void SaveSample(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                storageBox.Text))
        {
            MessageBox.Show(
                "Введите местонахождение");

            return;
        }

        if (_sample == null)
        {
            var sample = new Sample
            {
                VirusId =
                    (int)virusBox.SelectedValue,

                ResponsibleScientistId =
                    (int?)scientistBox.SelectedValue,

                StorageLocation =
                    storageBox.Text,

                CreatedAt =
                    createdPicker.Value,

                Notes =
                    notesBox.Text,

                IsDestroyed =
                    destroyedBox.Checked
            };

            _context.Samples.Add(sample);
        }
        else
        {
            _sample.VirusId =
                (int)virusBox.SelectedValue;

            _sample.ResponsibleScientistId =
                (int?)scientistBox.SelectedValue;

            _sample.StorageLocation =
                storageBox.Text;

            _sample.CreatedAt =
                createdPicker.Value;

            _sample.Notes =
                notesBox.Text;

            _sample.IsDestroyed =
                destroyedBox.Checked;
        }

        _context.SaveChanges();

        DialogResult =
            DialogResult.OK;

        Close();
    }
}