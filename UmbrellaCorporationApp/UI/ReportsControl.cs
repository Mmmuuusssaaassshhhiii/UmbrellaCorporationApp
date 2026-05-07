using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ReportsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

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

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(25, 0, 0)
        };

        Controls.Add(topPanel);

        var createBtn = CreateTopButton("СОЗДАТЬ ОТЧЕТ");

        createBtn.Location = new Point(10, 15);

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

        container = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(30, 0, 0)
        };

        Controls.Add(container);

        LoadReports();
    }

    private void LoadReports()
    {
        container.Controls.Clear();

        var reports = _context.LabReports
            .Include(x => x.Author)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        foreach (var report in reports)
        {
            var card = new ReportCard(report);

            var menu = new ContextMenuStrip();

            menu.Items.Add("Редактировать", null, (s, e) =>
            {
                if (report.AuthorId != _currentUser.Id)
                {
                    MessageBox.Show(
                        "Можно редактировать только свои отчеты");

                    return;
                }

                var editor = new ReportEditorForm(
                    _context,
                    _currentUser,
                    report);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadReports();
                }
            });

            menu.Items.Add("Удалить", null, (s, e) =>
            {
                if (report.AuthorId != _currentUser.Id)
                {
                    MessageBox.Show(
                        "Можно удалять только свои отчеты");

                    return;
                }

                var result = MessageBox.Show(
                    "Удалить отчет?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _context.LabReports.Remove(report);

                    _context.SaveChanges();

                    LoadReports();
                }
            });

            card.ContextMenuStrip = menu;

            container.Controls.Add(card);
        }
    }

    private Button CreateTopButton(string text)
    {
        var btn = new Button
        {
            Text = text,

            Width = 220,

            Height = 40,

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font("Exo 2", 10, FontStyle.Bold),

            Cursor = Cursors.Hand
        };

        btn.FlatAppearance.BorderSize = 0;

        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor = Color.FromArgb(180, 0, 0);
        };

        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor = Color.FromArgb(120, 0, 0);
        };

        return btn;
    }
}