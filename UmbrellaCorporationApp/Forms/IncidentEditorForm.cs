using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class IncidentEditorForm : Form
{
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
        Text = "Редактор инцидента";

        Size = new Size(900, 720);

        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.FromArgb(20, 0, 0);

        FormBorderStyle = FormBorderStyle.FixedSingle;

        typeBox = new TextBox
        {
            Location = new Point(30, 30),
            Width = 820,
            Font = new Font("Exo 2", 13)
        };

        Controls.Add(typeBox);

        locationBox = new TextBox
        {
            Location = new Point(30, 90),
            Width = 820,
            Font = new Font("Exo 2", 13)
        };

        Controls.Add(locationBox);

        severityBox = new ComboBox
        {
            Location = new Point(30, 150),

            Width = 250,

            DropDownStyle = ComboBoxStyle.DropDownList
        };

        severityBox.Items.AddRange(
            Enum.GetNames(typeof(IncidentSeverity)));

        severityBox.SelectedIndex = 0;

        Controls.Add(severityBox);

        resolvedBox = new CheckBox
        {
            Text = "Инцидент устранен",

            ForeColor = Color.White,

            BackColor = Color.Transparent,

            Font = new Font("Exo 2", 11),

            Location = new Point(320, 150),

            Width = 250
        };

        Controls.Add(resolvedBox);

        descriptionBox = new RichTextBox
        {
            Location = new Point(30, 220),

            Size = new Size(820, 380),

            Font = new Font("Exo 2", 12),

            BackColor = Color.FromArgb(40, 0, 0),

            ForeColor = Color.White
        };

        Controls.Add(descriptionBox);

        var saveBtn = new Button
        {
            Text = "СОХРАНИТЬ",

            Width = 220,

            Height = 45,

            Location = new Point(630, 620),

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font("Exo 2", 10, FontStyle.Bold)
        };

        saveBtn.FlatAppearance.BorderSize = 0;

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
}