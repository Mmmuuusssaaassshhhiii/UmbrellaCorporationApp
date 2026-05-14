using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.Forms;

public class SubjectEditorForm : Form
{
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
        Text = "Редактор испытуемого";

        Size = new Size(900, 760);

        StartPosition = FormStartPosition.CenterScreen;

        FormBorderStyle = FormBorderStyle.FixedSingle;

        BackColor = Color.FromArgb(20, 0, 0);

        MaximizeBox = false;

        InitializeInputs();

        InitializeSaveButton();
    }

    private void InitializeInputs()
    {
        codeBox = CreateTextBox(
            "КОД ИСПЫТУЕМОГО",
            new Point(30, 30));

        // ================= VIRUS =================

        virusBox = new ComboBox
        {
            Location = new Point(30, 100),

            Width = 820,

            Height = 40,

            Font = new Font("Exo 2", 13),

            BackColor = Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var viruses = _context.Viruses
            .Select(x => x.Name)
            .ToList();

        virusBox.Items.AddRange(viruses.ToArray());

        Controls.Add(virusBox);

        // ================= STATUS =================

        statusBox = new ComboBox
        {
            Location = new Point(30, 170),

            Width = 820,

            Height = 40,

            Font = new Font("Exo 2", 13),

            BackColor = Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            DropDownStyle = ComboBoxStyle.DropDownList
        };

        statusBox.Items.AddRange(
            Enum.GetNames(typeof(SubjectStatus)));

        Controls.Add(statusBox);

        // ================= LOCATION =================

        locationBox = CreateTextBox(
            "МЕСТОНАХОЖДЕНИЕ",
            new Point(30, 240));

        // ================= NOTES =================

        notesBox = new RichTextBox
        {
            Location = new Point(30, 320),

            Size = new Size(820, 320),

            Font = new Font("Consolas", 12),

            BackColor = Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            BorderStyle = BorderStyle.None
        };

        Controls.Add(notesBox);
    }

    private TextBox CreateTextBox(
        string placeholder,
        Point location)
    {
        var box = new TextBox
        {
            Width = 820,

            Height = 40,

            Location = location,

            Font = new Font("Exo 2", 13),

            BackColor = Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            BorderStyle = BorderStyle.FixedSingle,

            Text = placeholder,

            Tag = placeholder
        };

        box.GotFocus += (s, e) =>
        {
            if (box.Text == placeholder)
            {
                box.Text = "";
            }
        };

        box.LostFocus += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                box.Text = placeholder;
            }
        };

        Controls.Add(box);

        return box;
    }

    private void InitializeSaveButton()
    {
        var saveBtn = new Button
        {
            Text = "СОХРАНИТЬ",

            Width = 220,

            Height = 45,

            Location = new Point(630, 670),

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