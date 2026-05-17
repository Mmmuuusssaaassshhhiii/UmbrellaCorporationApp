using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.Forms;

public partial class FileEditorForm : Form
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
    private readonly ClassifiedFile? _file;

    private TextBox titleBox = null!;
    private ComboBox levelBox = null!;
    private ComboBox authorBox = null!;
    private RichTextBox contentBox = null!;

    public FileEditorForm(UmbrellaDbContext context, ClassifiedFile? file = null)
    {
        _context = context;
        _file = file;

        InitializeUI();

        LoadComboData();

        if (_file != null)
            LoadFile();
    }

    private void InitializeUI()
    {
        Size = new Size(850, 700);
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
        closeBtn.MouseEnter += (s, e) => closeBtn.BackColor = Color.DarkRed;
        closeBtn.MouseLeave += (s, e) => closeBtn.BackColor = Color.FromArgb(55, 0, 0);
        closeBtn.Click += (s, e) => Close();

        topBar.Controls.Add(closeBtn);

        // ================= TITLE =================
        var title = new Label
        {
            Text = _file == null ? "НОВЫЙ ФАЙЛ" : "РЕДАКТИРОВАНИЕ ФАЙЛА",
            Font = new Font("Exo 2", 22, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 70)
        };

        Controls.Add(title);

        int left = 30;
        int width = 760;
        int y = 130;

        // ================= TITLE FIELD =================
        Controls.Add(CreateLabel("ЗАГОЛОВОК", left, y));

        titleBox = CreateTextBox(left, y + 30, width);
        Controls.Add(titleBox);

        // ================= LEVEL + AUTHOR =================
        y += 90;

        Controls.Add(CreateLabel("УРОВЕНЬ ДОСТУПА", left, y));

        levelBox = CreateComboBox(left, y + 30, 250);
        levelBox.Items.AddRange(Enum.GetNames(typeof(FileLevel)));
        levelBox.SelectedIndex = 0;

        Controls.Add(levelBox);

        authorBox = CreateComboBox(left + 280, y + 30, 480);
        Controls.Add(authorBox);

        // ================= CONTENT =================
        y += 90;

        Controls.Add(CreateLabel("СОДЕРЖАНИЕ", left, y));

        contentBox = new RichTextBox
        {
            Location = new Point(left, y + 30),
            Size = new Size(width, 250),
            BackColor = Color.FromArgb(40, 0, 0),
            ForeColor = Color.White,
            Font = new Font("Exo 2", 11),
            BorderStyle = BorderStyle.FixedSingle
        };

        Controls.Add(contentBox);

        // ================= SAVE =================
        var saveBtn = new Button
        {
            Text = _file == null ? "СОХРАНИТЬ" : "ОБНОВИТЬ",
            Width = 220,
            Height = 45,
            Location = new Point(600, 610),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(120, 0, 0),
            ForeColor = Color.White,
            Font = new Font("Exo 2", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        saveBtn.FlatAppearance.BorderSize = 0;
        saveBtn.MouseEnter += (s, e) => saveBtn.BackColor = Color.FromArgb(180, 0, 0);
        saveBtn.MouseLeave += (s, e) => saveBtn.BackColor = Color.FromArgb(120, 0, 0);
        saveBtn.Click += SaveFile;

        Controls.Add(saveBtn);
    }

    private void LoadComboData()
    {
        authorBox.DataSource = _context.Employees.ToList();
        authorBox.DisplayMember = "FullName";
        authorBox.ValueMember = "Id";
    }

    private void LoadFile()
    {
        titleBox.Text = _file!.Title;
        levelBox.SelectedItem = _file.Level.ToString();
        authorBox.SelectedValue = _file.AuthorId;
        contentBox.Text = _file.Content;
    }

    private void SaveFile(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleBox.Text) ||
            string.IsNullOrWhiteSpace(contentBox.Text))
        {
            MessageBox.Show("Заполни все обязательные поля");
            return;
        }

        if (_file == null)
        {
            var file = new ClassifiedFile
            {
                Title = titleBox.Text,
                Level = Enum.Parse<FileLevel>(levelBox.SelectedItem!.ToString()!),
                Content = contentBox.Text,
                AuthorId = (int)authorBox.SelectedValue,
                CreatedAt = DateTime.Now
            };

            _context.Add(file);
        }
        else
        {
            _file.Title = titleBox.Text;
            _file.Level = Enum.Parse<FileLevel>(levelBox.SelectedItem!.ToString()!);
            _file.Content = contentBox.Text;
            _file.AuthorId = (int)authorBox.SelectedValue;
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;
        Close();
    }

    // ================= HELPERS =================
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
}