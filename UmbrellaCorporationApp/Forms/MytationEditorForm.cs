using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class MutationEditorForm : Form
{
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
        Text = "Редактор мутаций";

        Size = new Size(900, 650);

        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.FromArgb(20, 0, 0);

        subjectBox = new ComboBox
        {
            Location = new Point(30, 30),

            Width = 350,

            DropDownStyle = ComboBoxStyle.DropDownList
        };

        subjectBox.DataSource = _context.TestSubjects.ToList();

        subjectBox.DisplayMember = "Code";

        subjectBox.ValueMember = "Id";

        Controls.Add(subjectBox);

        virusBox = new ComboBox
        {
            Location = new Point(430, 30),

            Width = 350,

            DropDownStyle = ComboBoxStyle.DropDownList
        };

        virusBox.DataSource = _context.Viruses.ToList();

        virusBox.DisplayMember = "Name";

        virusBox.ValueMember = "Id";

        Controls.Add(virusBox);

        descriptionBox = new RichTextBox
        {
            Location = new Point(30, 100),

            Size = new Size(820, 420),

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

            Location = new Point(630, 550),

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font("Exo 2", 10, FontStyle.Bold)
        };

        saveBtn.FlatAppearance.BorderSize = 0;

        saveBtn.Click += SaveMutation;

        Controls.Add(saveBtn);
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