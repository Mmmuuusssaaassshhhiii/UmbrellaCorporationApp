using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ProtocolEditorForm : Form
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

    private readonly EmergencyProtocol? _protocol;

    private TextBox codeBox = null!;

    private TextBox nameBox = null!;

    private RichTextBox instructionsBox = null!;

    private CheckBox activeBox = null!;

    public ProtocolEditorForm(
        UmbrellaDbContext context,
        Employee currentUser,
        EmergencyProtocol? protocol = null)
    {
        _context = context;

        _currentUser = currentUser;

        _protocol = protocol;

        InitializeUI();

        if (_protocol != null)
        {
            LoadProtocol();
        }
    }

    private void InitializeUI()
{
    Size = new Size(900, 760);

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
        Text = _protocol == null
            ? "НОВЫЙ ПРОТОКОЛ"
            : "РЕДАКТИРОВАНИЕ ПРОТОКОЛА",

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

    Controls.Add(CreateLabel(
        "КОД ПРОТОКОЛА",
        30,
        140));

    codeBox = CreateTextBox(
        30,
        170,
        300);

    Controls.Add(codeBox);

    // ================= NAME =================

    Controls.Add(CreateLabel(
        "НАЗВАНИЕ ПРОТОКОЛА",
        360,
        140));

    nameBox = CreateTextBox(
        360,
        170,
        490);

    Controls.Add(nameBox);

    // ================= ACTIVE =================

    activeBox = new CheckBox
    {
        Text = "Протокол активен",

        ForeColor = Color.White,

        BackColor = Color.Transparent,

        Font = new Font(
            "Exo 2",
            11,
            FontStyle.Bold),

        Location = new Point(30, 240),

        AutoSize = true
    };

    Controls.Add(activeBox);

    // ================= INSTRUCTIONS =================

    Controls.Add(CreateLabel(
        "ИНСТРУКЦИИ",
        30,
        300));

    instructionsBox = new RichTextBox
    {
        Location = new Point(30, 330),

        Size = new Size(820, 280),

        Font = new Font(
            "Consolas",
            12),

        BackColor =
            Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle =
            BorderStyle.FixedSingle
    };

    Controls.Add(instructionsBox);

    // ================= SAVE BUTTON =================

    var saveBtn = new Button
    {
        Text = "СОХРАНИТЬ",

        Width = 220,

        Height = 45,

        Location = new Point(630, 660),

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

    saveBtn.Click += SaveProtocol;

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

private TextBox CreateTextBox(
    int x,
    int y,
    int width)
{
    return new TextBox
    {
        Location = new Point(x, y),

        Width = width,

        Height = 40,

        Font = new Font(
            "Exo 2",
            13),

        BackColor =
            Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle =
            BorderStyle.FixedSingle
    };
}

    private void LoadProtocol()
    {
        codeBox.Text = _protocol!.Code;

        nameBox.Text = _protocol.Name;

        instructionsBox.Text = _protocol.Instructions;

        activeBox.Checked = _protocol.IsActive;
    }

    private void SaveProtocol(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(codeBox.Text) ||
            string.IsNullOrWhiteSpace(nameBox.Text) ||
            string.IsNullOrWhiteSpace(instructionsBox.Text))
        {
            MessageBox.Show("Заполни поля");

            return;
        }

        if (_protocol == null)
        {
            var protocol = new EmergencyProtocol
            {
                Code = codeBox.Text,

                Name = nameBox.Text,

                Instructions = instructionsBox.Text,

                IsActive = activeBox.Checked
            };

            _context.EmergencyProtocols.Add(protocol);
        }
        else
        {
            _protocol.Code = codeBox.Text;

            _protocol.Name = nameBox.Text;

            _protocol.Instructions = instructionsBox.Text;

            _protocol.IsActive = activeBox.Checked;
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;

        Close();
    }
}