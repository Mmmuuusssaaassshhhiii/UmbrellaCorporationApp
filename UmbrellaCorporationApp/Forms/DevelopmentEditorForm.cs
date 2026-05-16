using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class DevelopmentEditorForm : Form
{
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
        Text = _development == null
            ? "Добавление разработки"
            : "Редактирование разработки";

        Size = new Size(700, 500);

        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.FromArgb(20, 0, 0);

        FormBorderStyle = FormBorderStyle.FixedSingle;

        MaximizeBox = false;

        var title = new Label
        {
            Text = _development == null
                ? "НОВАЯ РАЗРАБОТКА"
                : "РЕДАКТИРОВАНИЕ РАЗРАБОТКИ",

            Font = new Font(
                "Exo 2",
                20,
                FontStyle.Bold),

            ForeColor = Color.White,

            AutoSize = true,

            Location = new Point(30, 20)
        };

        Controls.Add(title);

        projectBox = CreateTextBox(30, 80);

        Controls.Add(projectBox);

        virusBox = CreateComboBox(30, 140);

        virusBox.DataSource =
            _context.Viruses.ToList();

        virusBox.DisplayMember = "Name";

        virusBox.ValueMember = "Id";

        Controls.Add(virusBox);

        scientistBox = CreateComboBox(30, 200);

        scientistBox.DataSource =
            _context.Employees.ToList();

        scientistBox.DisplayMember = "FullName";

        scientistBox.ValueMember = "Id";

        Controls.Add(scientistBox);

        statusBox = CreateComboBox(30, 260);

        statusBox.Items.AddRange(
            Enum.GetNames(
                typeof(DevelopmentStatus)));

        statusBox.SelectedIndex = 0;

        Controls.Add(statusBox);

        startPicker = new DateTimePicker
        {
            Location = new Point(30, 320),

            Width = 250,

            Font = new Font("Exo 2", 11)
        };

        Controls.Add(startPicker);

        endPicker = new DateTimePicker
        {
            Location = new Point(320, 320),

            Width = 250,

            Font = new Font("Exo 2", 11)
        };

        Controls.Add(endPicker);

        var saveBtn = new Button
        {
            Text = _development == null
                ? "СОХРАНИТЬ"
                : "ОБНОВИТЬ",

            Width = 220,

            Height = 45,

            Location = new Point(430, 390),

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font(
                "Exo 2",
                10,
                FontStyle.Bold)
        };

        saveBtn.FlatAppearance.BorderSize = 0;

        saveBtn.Click += SaveDevelopment;

        Controls.Add(saveBtn);
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

            Width = 600,

            Font = new Font("Exo 2", 12)
        };
    }

    private ComboBox CreateComboBox(
        int x,
        int y)
    {
        return new ComboBox
        {
            Location = new Point(x, y),

            Width = 600,

            DropDownStyle =
                ComboBoxStyle.DropDownList,

            Font = new Font("Exo 2", 11)
        };
    }
}