using System;
using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class ReportEditorForm : Form
{
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private readonly LabReport? _report;

    private TextBox titleBox = null!;

    private RichTextBox contentBox = null!;

    private ComboBox levelBox = null!;

    public ReportEditorForm(
        UmbrellaDbContext context,
        Employee currentUser,
        LabReport? report = null)
    {
        _context = context;

        _currentUser = currentUser;

        _report = report;

        InitializeUI();

        if (_report != null)
        {
            LoadReport();
        }
    }

    private void InitializeUI()
    {
        Text = "Редактор отчета";

        Size = new Size(900, 700);

        StartPosition = FormStartPosition.CenterScreen;

        BackColor = Color.FromArgb(20, 0, 0);

        FormBorderStyle = FormBorderStyle.FixedSingle;

        titleBox = new TextBox
        {
            Location = new Point(30, 30),

            Width = 820,

            Font = new Font("Exo 2", 14)
        };

        Controls.Add(titleBox);

        levelBox = new ComboBox
        {
            Location = new Point(30, 80),

            Width = 250,

            DropDownStyle = ComboBoxStyle.DropDownList
        };

        levelBox.Items.AddRange(
            Enum.GetNames(typeof(ConfidentialityLevel)));

        levelBox.SelectedIndex = 0;

        Controls.Add(levelBox);

        contentBox = new RichTextBox
        {
            Location = new Point(30, 130),

            Size = new Size(820, 450),

            Font = new Font("Consolas", 12)
        };

        Controls.Add(contentBox);

        var saveBtn = new Button
        {
            Text = "СОХРАНИТЬ",

            Width = 220,

            Height = 45,

            Location = new Point(630, 600),

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font("Exo 2", 10, FontStyle.Bold)
        };

        saveBtn.FlatAppearance.BorderSize = 0;

        saveBtn.Click += SaveReport;

        Controls.Add(saveBtn);
    }

    private void LoadReport()
    {
        titleBox.Text = _report!.Title;

        contentBox.Text = _report.Content;

        levelBox.SelectedItem =
            _report.ConfidentialityLevel.ToString();
    }

    private void SaveReport(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleBox.Text) ||
            string.IsNullOrWhiteSpace(contentBox.Text))
        {
            MessageBox.Show("Заполни поля");

            return;
        }

        if (_report == null)
        {
            var report = new LabReport
            {
                AuthorId = _currentUser.Id,

                Title = titleBox.Text,

                Content = contentBox.Text,

                CreatedAt = DateTime.Now,

                ConfidentialityLevel =
                    Enum.Parse<ConfidentialityLevel>(
                        levelBox.SelectedItem!.ToString()!)
            };

            _context.LabReports.Add(report);
        }
        else
        {
            if (_report.AuthorId != _currentUser.Id)
            {
                MessageBox.Show(
                    "Можно редактировать только свои отчеты");

                return;
            }

            _report.Title = titleBox.Text;

            _report.Content = contentBox.Text;

            _report.ConfidentialityLevel =
                Enum.Parse<ConfidentialityLevel>(
                    levelBox.SelectedItem!.ToString()!);
        }

        _context.SaveChanges();

        DialogResult = DialogResult.OK;

        Close();
    }
}