using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class VirusEditorForm : Form
{
    private readonly UmbrellaDbContext
        _context;

    private readonly Virus? _virus;

    private TextBox nameBox = null!;

    private ComboBox dangerBox = null!;

    private NumericUpDown
        incubationBox = null!;

    private RichTextBox
        symptomsBox = null!;

    private CheckBox antidoteBox = null!;

    public VirusEditorForm(
        UmbrellaDbContext context,
        Virus? virus = null)
    {
        _context = context;

        _virus = virus;

        InitializeUI();

        if (_virus != null)
        {
            LoadVirus();
        }
    }

    private void InitializeUI()
    {
        Text = _virus == null
            ? "Добавление вируса"
            : "Редактирование вируса";

        Size = new Size(800, 650);

        StartPosition =
            FormStartPosition
                .CenterScreen;

        BackColor =
            Color.FromArgb(20, 0, 0);

        FormBorderStyle =
            FormBorderStyle.FixedSingle;

        nameBox = new TextBox
        {
            Location = new Point(30, 30),

            Width = 700,

            Font =
                new Font("Exo 2", 12)
        };

        Controls.Add(nameBox);

        dangerBox = new ComboBox
        {
            Location = new Point(30, 90),

            Width = 250,

            DropDownStyle =
                ComboBoxStyle
                    .DropDownList
        };

        dangerBox.Items.AddRange(
            Enum.GetNames(
                typeof(
                    VirusDangerLevel)));

        dangerBox.SelectedIndex = 0;

        Controls.Add(dangerBox);

        incubationBox =
            new NumericUpDown
            {
                Location =
                    new Point(320, 90),

                Width = 150,

                Maximum = 9999,

                Font =
                    new Font(
                        "Exo 2",
                        11)
            };

        Controls.Add(incubationBox);

        antidoteBox = new CheckBox
        {
            Text = "Есть антидот",

            Location =
                new Point(520, 92),

            ForeColor =
                Color.White,

            BackColor =
                Color.Transparent,

            Font =
                new Font(
                    "Exo 2",
                    11)
        };

        Controls.Add(antidoteBox);

        symptomsBox =
            new RichTextBox
            {
                Location =
                    new Point(30, 150),

                Size =
                    new Size(700, 340),

                BackColor =
                    Color.FromArgb(
                        40,
                        0,
                        0),

                ForeColor =
                    Color.White,

                Font =
                    new Font(
                        "Exo 2",
                        11)
            };

        Controls.Add(symptomsBox);

        var saveBtn = new Button
        {
            Text = "СОХРАНИТЬ",

            Width = 220,

            Height = 45,

            Location =
                new Point(510, 530),

            FlatStyle =
                FlatStyle.Flat,

            BackColor =
                Color.FromArgb(
                    120,
                    0,
                    0),

            ForeColor =
                Color.White,

            Font =
                new Font(
                    "Exo 2",
                    10,
                    FontStyle.Bold)
        };

        saveBtn.FlatAppearance
            .BorderSize = 0;

        saveBtn.Click += SaveVirus;

        Controls.Add(saveBtn);
    }

    private void LoadVirus()
    {
        nameBox.Text = _virus!.Name;

        dangerBox.SelectedItem =
            _virus.DangerLevel
                .ToString();

        incubationBox.Value =
            _virus.IncubationHours;

        symptomsBox.Text =
            _virus.Symptoms;

        antidoteBox.Checked =
            _virus.AntidoteExists;
    }

    private void SaveVirus(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                nameBox.Text))
        {
            MessageBox.Show(
                "Введите название");

            return;
        }

        if (_virus == null)
        {
            var virus = new Virus
            {
                Name = nameBox.Text,

                DangerLevel =
                    Enum.Parse
                        <VirusDangerLevel>(
                            dangerBox
                                .SelectedItem!
                                .ToString()!),

                IncubationHours =
                    (int)
                    incubationBox
                        .Value,

                Symptoms =
                    symptomsBox.Text,

                AntidoteExists =
                    antidoteBox.Checked
            };

            _context.Viruses
                .Add(virus);
        }
        else
        {
            _virus.Name =
                nameBox.Text;

            _virus.DangerLevel =
                Enum.Parse
                    <VirusDangerLevel>(
                        dangerBox
                            .SelectedItem!
                            .ToString()!);

            _virus.IncubationHours =
                (int)
                incubationBox
                    .Value;

            _virus.Symptoms =
                symptomsBox.Text;

            _virus.AntidoteExists =
                antidoteBox.Checked;
        }

        _context.SaveChanges();

        DialogResult =
            DialogResult.OK;

        Close();
    }
}