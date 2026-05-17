using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class MutationEditorForm : Form
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

    private readonly Mutation? _mutation;

    private ComboBox subjectBox = null!;

    private ComboBox virusBox = null!;

    private RichTextBox descriptionBox = null!;

    public MutationEditorForm(
        UmbrellaDbContext context,
        Employee currentUser,
        Mutation? mutation = null)
    {
        _context = context;

        _currentUser = currentUser;

        _mutation = mutation;

        InitializeUI();

        if (_mutation != null)
        {
            LoadMutation();
        }
    }

    private void InitializeUI()
{
    Size = new Size(900, 720);

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
        Text = _mutation == null
            ? "НОВАЯ МУТАЦИЯ"
            : "РЕДАКТИРОВАНИЕ МУТАЦИИ",

        Font = new Font(
            "Exo 2",
            22,
            FontStyle.Bold),

        ForeColor = Color.White,

        AutoSize = true,

        Location = new Point(30, 70)
    };

    Controls.Add(title);

    // ================= SUBJECT =================

    Controls.Add(CreateLabel(
        "ИСПЫТУЕМЫЙ",
        30,
        140));

    subjectBox = CreateComboBox(
        30,
        170);

    subjectBox.DataSource =
        _context.TestSubjects.ToList();

    subjectBox.DisplayMember = "Code";

    subjectBox.ValueMember = "Id";

    Controls.Add(subjectBox);

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

    // ================= DESCRIPTION =================

    Controls.Add(CreateLabel(
        "ОПИСАНИЕ МУТАЦИИ",
        30,
        340));

    descriptionBox = new RichTextBox
    {
        Location = new Point(30, 370),

        Size = new Size(820, 220),

        Font = new Font(
            "Exo 2",
            12),

        BackColor =
            Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        BorderStyle =
            BorderStyle.FixedSingle
    };

    Controls.Add(descriptionBox);

    // ================= SAVE BUTTON =================

    var saveBtn = new Button
    {
        Text = "СОХРАНИТЬ",

        Width = 220,

        Height = 45,

        Location = new Point(630, 630),

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

    saveBtn.Click += SaveMutation;

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

private ComboBox CreateComboBox(
    int x,
    int y)
{
    return new ComboBox
    {
        Location = new Point(x, y),

        Width = 820,

        Height = 40,

        Font = new Font(
            "Exo 2",
            13),

        BackColor =
            Color.FromArgb(40, 0, 0),

        ForeColor = Color.White,

        DropDownStyle =
            ComboBoxStyle.DropDownList
    };
}

    private void LoadMutation()
    {
        subjectBox.SelectedValue = _mutation!.TestSubjectId;

        virusBox.SelectedValue = _mutation.VirusId;

        descriptionBox.Text = _mutation.ChangeDescription;
    }

    private void SaveMutation(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(descriptionBox.Text))
        {
            MessageBox.Show("Заполни описание");

            return;
        }

        if (_mutation == null)
        {
            var mutation = new Mutation
            {
                TestSubjectId =
                    (int)subjectBox.SelectedValue,

                VirusId =
                    (int)virusBox.SelectedValue,

                ChangeDescription =
                    descriptionBox.Text,

                ObservedAt = DateTime.Now
            };

            _context.Mutations.Add(mutation);
        }
        else
        {
            _mutation.TestSubjectId =
                (int)subjectBox.SelectedValue;

            _mutation.VirusId =
                (int)virusBox.SelectedValue;

            _mutation.ChangeDescription =
                descriptionBox.Text;
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;

        Close();
    }
}