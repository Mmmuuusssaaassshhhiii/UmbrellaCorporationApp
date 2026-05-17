using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class ReportsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

    private TextBox searchBox = null!;

    private ComboBox authorFilter = null!;

    private ComboBox levelFilter = null!;

    private ComboBox sortBox = null!;

    public ReportsControl(
        UmbrellaDbContext context,
        Employee currentUser)
    {
        _context = context;

        _currentUser = currentUser;

        InitializeUI();
    }

    private void InitializeUI()
    {
        Dock = DockStyle.Fill;

        BackColor = Color.FromArgb(30, 0, 0);

        // ================= TOP PANEL =================

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,

            Height = 80,

            BackColor = Color.FromArgb(25, 0, 0)
        };

        Controls.Add(topPanel);

        // ================= CREATE BUTTON =================

        if (_currentUser.ClearanceLevel == ClearanceLevel.Level10)
        {
            var createBtn = CreateTopButton("СОЗДАТЬ ОТЧЕТ");

            createBtn.Location = new Point(20, 22);

            createBtn.Click += (s, e) =>
            {
                var form = new ReportEditorForm(
                    _context,
                    _currentUser);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadReports();
                }
            };

            topPanel.Controls.Add(createBtn);
        }

        // ================= FILTER PANEL =================

        var filtersPanel = new FlowLayoutPanel
        {
            Location = new Point(260, 20),

            Size = new Size(1400, 45),

            FlowDirection =
                FlowDirection.LeftToRight,

            WrapContents = false,

            AutoScroll = false,

            BackColor = Color.Transparent
        };

        topPanel.Controls.Add(filtersPanel);

        // ================= SEARCH =================

        searchBox = new TextBox
        {
            PlaceholderText =
                "Поиск отчета...",

            Width = 260,

            Height = 40,

            Font = new Font(
                "Exo 2",
                11),

            BackColor =
                Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            BorderStyle =
                BorderStyle.FixedSingle,

            Margin = new Padding(0, 0, 10, 0)
        };

        searchBox.TextChanged +=
            (s, e) => LoadReports();

        filtersPanel.Controls.Add(searchBox);

        // ================= AUTHOR FILTER =================

        authorFilter = new ComboBox
        {
            Width = 220,
            
            Height = 40,

            DropDownStyle =
                ComboBoxStyle.DropDownList,

            Font = new Font(
                "Exo 2",
                10),

            BackColor =
                Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            Margin = new Padding(0, 0, 10, 0)
        };

        authorFilter.Items.Add(
            "ВСЕ АВТОРЫ");

        foreach (var employee
                 in _context.Employees
                     .OrderBy(x => x.FullName))
        {
            authorFilter.Items.Add(
                employee.FullName);
        }

        authorFilter.SelectedIndex = 0;

        authorFilter.SelectedIndexChanged +=
            (s, e) => LoadReports();

        filtersPanel.Controls.Add(authorFilter);

        // ================= LEVEL FILTER =================

        levelFilter = new ComboBox
        {
            Width = 190,
            
            Height = 40,

            DropDownStyle =
                ComboBoxStyle.DropDownList,

            Font = new Font(
                "Exo 2",
                10),

            BackColor =
                Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            Margin = new Padding(0, 0, 10, 0)
        };

        levelFilter.Items.Add(
            "ВСЕ УРОВНИ");

        foreach (var level
                 in Enum.GetNames(
                     typeof(
                         ConfidentialityLevel)))
        {
            levelFilter.Items.Add(level);
        }

        levelFilter.SelectedIndex = 0;

        levelFilter.SelectedIndexChanged +=
            (s, e) => LoadReports();

        filtersPanel.Controls.Add(levelFilter);

        // ================= SORT =================

        sortBox = new ComboBox
        {
            Width = 220,
            
            Height = 40,

            DropDownStyle =
                ComboBoxStyle.DropDownList,

            Font = new Font(
                "Exo 2",
                10),

            BackColor =
                Color.FromArgb(40, 0, 0),

            ForeColor = Color.White
        };

        sortBox.Items.AddRange(new[]
        {
            "Сначала новые",
            "Сначала старые",
            "По названию"
        });

        sortBox.SelectedIndex = 0;

        sortBox.SelectedIndexChanged +=
            (s, e) => LoadReports();

        filtersPanel.Controls.Add(sortBox);

        // ================= REPORTS CONTAINER =================

        container = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,

            AutoScroll = true,

            Padding = new Padding(20),

            BackColor =
                Color.FromArgb(30, 0, 0),

            WrapContents = true
        };

        Controls.Add(container);

        LoadReports();
    }

    private void LoadReports()
    {
        container.SuspendLayout();

        container.Controls.Clear();

        var query = _context.LabReports
            .Include(x => x.Author)
            .AsQueryable();

        // ================= SEARCH =================

        if (!string.IsNullOrWhiteSpace(
                searchBox.Text))
        {
            var search =
                searchBox.Text.ToLower();

            query = query.Where(x =>
                x.Title.ToLower()
                    .Contains(search) ||

                x.Content.ToLower()
                    .Contains(search));
        }

        // ================= AUTHOR FILTER =================

        if (authorFilter.SelectedIndex > 0)
        {
            var author =
                authorFilter.SelectedItem!
                    .ToString();

            query = query.Where(x =>
                x.Author != null &&
                x.Author.FullName == author);
        }

        // ================= LEVEL FILTER =================

        if (levelFilter.SelectedIndex > 0)
        {
            var level =
                Enum.Parse
                    <ConfidentialityLevel>(
                        levelFilter
                            .SelectedItem!
                            .ToString()!);

            query = query.Where(x =>
                x.ConfidentialityLevel
                    == level);
        }

        // ================= SORT =================

        switch (sortBox.SelectedIndex)
        {
            case 0:

                query = query.OrderByDescending(
                    x => x.CreatedAt);

                break;

            case 1:

                query = query.OrderBy(
                    x => x.CreatedAt);

                break;

            case 2:

                query = query.OrderBy(
                    x => x.Title);

                break;
        }

        var reports = query.ToList();

        foreach (var report in reports)
        {
            var card = new ReportCard(report);

            var menu = new ContextMenuStrip();

            menu.Items.Add(
                "Редактировать",
                null,
                (s, e) =>
                {
                    if (report.AuthorId
                        != _currentUser.Id)
                    {
                        MessageBox.Show(
                            "Можно редактировать только свои отчеты");

                        return;
                    }

                    var editor =
                        new ReportEditorForm(
                            _context,
                            _currentUser,
                            report);

                    if (editor.ShowDialog()
                        == DialogResult.OK)
                    {
                        LoadReports();
                    }
                });

            menu.Items.Add(
                "Удалить",
                null,
                (s, e) =>
                {
                    if (report.AuthorId
                        != _currentUser.Id)
                    {
                        MessageBox.Show(
                            "Можно удалять только свои отчеты");

                        return;
                    }

                    var result = MessageBox.Show(
                        "Удалить отчет?",
                        "Подтверждение",
                        MessageBoxButtons.YesNo);

                    if (result
                        == DialogResult.Yes)
                    {
                        _context.LabReports
                            .Remove(report);

                        _context.SaveChanges();

                        LoadReports();
                    }
                });

            card.ContextMenuStrip = menu;

            container.Controls.Add(card);
        }

        container.ResumeLayout();
    }

    private Button CreateTopButton(string text)
    {
        var btn = new Button
        {
            Text = text,

            Width = 220,

            Height = 40,

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

        btn.FlatAppearance.BorderSize = 0;

        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor =
                Color.FromArgb(180, 0, 0);
        };

        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor =
                Color.FromArgb(120, 0, 0);
        };

        return btn;
    }
}