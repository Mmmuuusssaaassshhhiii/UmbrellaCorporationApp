using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.Forms;

public class SubjectEditorForm : Form
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

    private readonly TestSubject? _subject;

    private TextBox codeBox = null!;
    private ComboBox virusBox = null!;
    private ComboBox statusBox = null!;
    private TextBox locationBox = null!;
    private RichTextBox notesBox = null!;

    public SubjectEditorForm(
        UmbrellaDbContext context,
        TestSubject? subject = null)
    {
        _context = context;

        _subject = subject;

        InitializeUI();

        if (_subject != null)
        {
            LoadSubject();
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
        Text = _subject == null
            ? "НОВЫЙ ИСПЫТУЕМЫЙ"
            : "РЕДАКТИРОВАНИЕ ИСПЫТУЕМОГО",

        Font = new Font(
            "Exo 2",
            22,
            FontStyle.Bold),

        ForeColor = Color.White,

        AutoSize = true,

        Location = new Point(30, 70)
    };

    Controls.Add(title);

    // ================= CODE =================

    var codeLabel = CreateLabel(
        "КОД ИСПЫТУЕМОГО",
        30,
        140);

    Controls.Add(codeLabel);

    codeBox = CreateTextBox(
        30,
        170);

    Controls.Add(codeBox);

    // ================= VIRUS =================

    var virusLabel = CreateLabel(
        "ВИРУС",
        30,
        240);

    Controls.Add(virusLabel);

    virusBox = CreateComboBox(
        30,
        270);

    virusBox.DataSource =
        _context.Viruses.ToList();

    virusBox.DisplayMember = "Name";

    virusBox.ValueMember = "Id";

    Controls.Add(virusBox);

    // ================= STATUS =================

    var statusLabel = CreateLabel(
        "СТАТУС",
        30,
        340);

    Controls.Add(statusLabel);

    statusBox = CreateComboBox(
        30,
        370);

    statusBox.Items.AddRange(
        Enum.GetNames(
            typeof(SubjectStatus)));

    Controls.Add(statusBox);

    // ================= LOCATION =================

    var locationLabel = CreateLabel(
        "МЕСТОНАХОЖДЕНИЕ",
        30,
        440);

    Controls.Add(locationLabel);

    locationBox = CreateTextBox(
        30,
        470);

    Controls.Add(locationBox);

    // ================= NOTES =================

    var notesLabel = CreateLabel(
        "ЗАМЕТКИ",
        30,
        540);

    Controls.Add(notesLabel);

    notesBox = new RichTextBox
    {
        Location = new Point(30, 570),

        Size = new Size(820, 100),

        Font = new Font("Exo 2", 12),

        BackColor = Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle = BorderStyle.FixedSingle
    };

    Controls.Add(notesBox);

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

            Location = new Point(630, 690),

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font("Exo 2", 10, FontStyle.Bold),

            Cursor = Cursors.Hand
        };

        saveBtn.FlatAppearance.BorderSize = 0;

        saveBtn.MouseEnter += (s, e) =>
        {
            saveBtn.BackColor = Color.FromArgb(180, 0, 0);
        };

        saveBtn.MouseLeave += (s, e) =>
        {
            saveBtn.BackColor = Color.FromArgb(120, 0, 0);
        };

        saveBtn.Click += SaveSubject;

        Controls.Add(saveBtn);
    }

    private void LoadSubject()
    {
        codeBox.Text = _subject!.Code;

        virusBox.SelectedItem = _subject.Virus?.Name;

        statusBox.SelectedItem =
            _subject.Status.ToString();

        locationBox.Text = _subject.Location;

        notesBox.Text = _subject.Notes;
    }

    private void SaveSubject(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(codeBox.Text))
        {
            MessageBox.Show("Введите код");

            return;
        }

        if (virusBox.SelectedItem == null)
        {
            MessageBox.Show("Выберите вирус");

            return;
        }

        if (statusBox.SelectedItem == null)
        {
            MessageBox.Show("Выберите статус");

            return;
        }

        var selectedVirus = _context.Viruses
            .FirstOrDefault(x =>
                x.Name == virusBox.SelectedItem.ToString());

        if (selectedVirus == null)
        {
            MessageBox.Show("Вирус не найден");

            return;
        }

        var parsedStatus =
            Enum.Parse<SubjectStatus>(
                statusBox.SelectedItem.ToString()!);

        if (_subject == null)
        {
            var subject = new TestSubject
            {
                Code = codeBox.Text,

                VirusId = selectedVirus.Id,

                Status = parsedStatus,

                Location = locationBox.Text,

                AcquiredDate = DateTime.Now,

                Notes = notesBox.Text
            };

            _context.TestSubjects.Add(subject);
        }
        else
        {
            _subject.Code = codeBox.Text;

            _subject.VirusId = selectedVirus.Id;

            _subject.Status = parsedStatus;

            _subject.Location = locationBox.Text;

            _subject.Notes = notesBox.Text;
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;

        Close();
    }
}