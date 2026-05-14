using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ProtocolEditorForm : Form
{
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
        Text = "Редактор протоколов";

        Size = new Size(900, 700);

        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.FromArgb(20, 0, 0);

        codeBox = new TextBox
        {
            Location = new Point(30, 30),

            Width = 300,

            Font = new Font("Exo 2", 13)
        };

        Controls.Add(codeBox);

        nameBox = new TextBox
        {
            Location = new Point(360, 30),

            Width = 490,

            Font = new Font("Exo 2", 13)
        };

        Controls.Add(nameBox);

        activeBox = new CheckBox
        {
            Text = "Протокол активен",

            ForeColor = Color.White,

            BackColor = Color.Transparent,

            Font = new Font("Exo 2", 11),

            Location = new Point(30, 80),

            Width = 250
        };

        Controls.Add(activeBox);

        instructionsBox = new RichTextBox
        {
            Location = new Point(30, 130),

            Size = new Size(820, 450),

            Font = new Font("Consolas", 12),

            BackColor = Color.FromArgb(40, 0, 0),

            ForeColor = Color.White
        };

        Controls.Add(instructionsBox);

        var saveBtn = new Button
        {
            Text = "СОХРАНИТЬ",

            Width = 220,

            Height = 45,

            Location = new Point(630, 610),

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font("Exo 2", 10, FontStyle.Bold)
        };

        saveBtn.FlatAppearance.BorderSize = 0;

        saveBtn.Click += SaveProtocol;

        Controls.Add(saveBtn);
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