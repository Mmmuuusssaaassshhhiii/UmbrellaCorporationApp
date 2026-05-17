using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class VirusEditorForm : Form
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
    private readonly Virus? _virus;

    private TextBox nameBox = null!;
    private ComboBox dangerBox = null!;
    private NumericUpDown incubationBox = null!;
    private RichTextBox symptomsBox = null!;
    private CheckBox antidoteBox = null!;

    public VirusEditorForm(UmbrellaDbContext context, Virus? virus = null)
    {
        _context = context;
        _virus = virus;

        InitializeUI();

        if (_virus != null)
            LoadVirus();
    }

    private void InitializeUI()
    {
        Size = new Size(800, 650);

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

                SendMessage(Handle, 0xA1, 0x2, 0);
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
            closeBtn.BackColor = Color.DarkRed;

        closeBtn.MouseLeave += (s, e) =>
            closeBtn.BackColor = Color.FromArgb(55, 0, 0);

        closeBtn.Click += (s, e) => Close();

        topBar.Controls.Add(closeBtn);

        // ================= TITLE =================
        var title = new Label
        {
            Text = _virus == null ? "НОВЫЙ ВИРУС" : "РЕДАКТИРОВАНИЕ ВИРУСА",
            Font = new Font("Exo 2", 22, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 70)
        };

        Controls.Add(title);

        int left = 30;
        int width = 700;
        int y = 130;

        // ================= NAME =================
        Controls.Add(CreateLabel("НАЗВАНИЕ", left, y));

        nameBox = CreateTextBox(left, y + 30, width);
        Controls.Add(nameBox);

        // ================= DANGER + INCUBATION =================
        y += 90;

        Controls.Add(CreateLabel("УРОВЕНЬ ОПАСНОСТИ", left, y));

        dangerBox = CreateComboBox(left, y + 30, 250);

        dangerBox.Items.AddRange(Enum.GetNames(typeof(VirusDangerLevel)));
        dangerBox.SelectedIndex = 0;

        Controls.Add(dangerBox);

        incubationBox = new NumericUpDown
        {
            Location = new Point(300, y + 30),
            Width = 150,
            Maximum = 9999,
            Font = new Font("Exo 2", 11),
            BackColor = Color.FromArgb(40, 0, 0),
            ForeColor = Color.White
        };

        Controls.Add(incubationBox);

        antidoteBox = new CheckBox
        {
            Text = "Есть антидот",
            Location = new Point(480, y + 32),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            Font = new Font("Exo 2", 11),
            AutoSize = true
        };

        Controls.Add(antidoteBox);

        // ================= SYMPTOMS =================
        y += 90;

        Controls.Add(CreateLabel("СИМПТОМЫ", left, y));

        symptomsBox = new RichTextBox
        {
            Location = new Point(left, y + 30),
            Size = new Size(width, 220),
            BackColor = Color.FromArgb(40, 0, 0),
            ForeColor = Color.White,
            Font = new Font("Exo 2", 11),
            BorderStyle = BorderStyle.FixedSingle
        };

        Controls.Add(symptomsBox);

        // ================= SAVE BUTTON =================
        var saveBtn = new Button
        {
            Text = _virus == null ? "СОХРАНИТЬ" : "ОБНОВИТЬ",
            Width = 220,
            Height = 45,
            Location = new Point(510, 580),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(120, 0, 0),
            ForeColor = Color.White,
            Font = new Font("Exo 2", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        saveBtn.FlatAppearance.BorderSize = 0;

        saveBtn.MouseEnter += (s, e) =>
            saveBtn.BackColor = Color.FromArgb(180, 0, 0);

        saveBtn.MouseLeave += (s, e) =>
            saveBtn.BackColor = Color.FromArgb(120, 0, 0);

        saveBtn.Click += SaveVirus;

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
            Font = new Font("Exo 2", 10, FontStyle.Bold)
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

    private void LoadVirus()
    {
        nameBox.Text = _virus!.Name;
        dangerBox.SelectedItem = _virus.DangerLevel.ToString();
        incubationBox.Value = _virus.IncubationHours;
        symptomsBox.Text = _virus.Symptoms;
        antidoteBox.Checked = _virus.AntidoteExists;
    }

    private void SaveVirus(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameBox.Text))
        {
            MessageBox.Show("Введите название");
            return;
        }

        if (_virus == null)
        {
            var virus = new Virus
            {
                Name = nameBox.Text,
                DangerLevel = Enum.Parse<VirusDangerLevel>(dangerBox.SelectedItem!.ToString()!),
                IncubationHours = (int)incubationBox.Value,
                Symptoms = symptomsBox.Text,
                AntidoteExists = antidoteBox.Checked
            };

            _context.Viruses.Add(virus);
        }
        else
        {
            _virus.Name = nameBox.Text;
            _virus.DangerLevel = Enum.Parse<VirusDangerLevel>(dangerBox.SelectedItem!.ToString()!);
            _virus.IncubationHours = (int)incubationBox.Value;
            _virus.Symptoms = symptomsBox.Text;
            _virus.AntidoteExists = antidoteBox.Checked;
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;
        Close();
    }
}