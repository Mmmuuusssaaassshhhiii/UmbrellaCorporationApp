using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorporationApp.UI;

public class LogsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

    public LogsControl(
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

        if (_currentUser.ClearanceLevel == ClearanceLevel.Level10)
        {
            var createBtn = CreateTopButton("СОЗДАТЬ ЗАПИСЬ");

            createBtn.Location = new Point(10, 15);

            createBtn.Click += (s, e) =>
            {
                var form = new IncidentEditorForm(
                    _context,
                    _currentUser);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadLogs();
                }
            };

            topPanel.Controls.Add(createBtn);
        }
        
        container = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(30, 0, 0)
        };

        Controls.Add(container);

        LoadLogs();
    }

    private void LoadLogs()
    {
        container.Controls.Clear();

        var logs = _context.IncidentLogs
            .Include(x => x.ReportedBy)
            .OrderByDescending(x => x.OccurredAt)
            .ToList();

        foreach (var log in logs)
        {
            var card = new IncidentCard(log);

            if (_currentUser.ClearanceLevel == ClearanceLevel.Level10)
            {
                var menu = new ContextMenuStrip();

                menu.Items.Add("Редактировать", null, (s, e) =>
                {
                    if (log.ReportedById != _currentUser.Id)
                    {
                        MessageBox.Show(
                            "Можно редактировать только свои записи");

                        return;
                    }

                    var editor = new IncidentEditorForm(
                        _context,
                        _currentUser,
                        log);

                    if (editor.ShowDialog() == DialogResult.OK)
                    {
                        LoadLogs();
                    }
                });

                menu.Items.Add("Удалить", null, (s, e) =>
                {
                    if (log.ReportedById != _currentUser.Id)
                    {
                        MessageBox.Show(
                            "Можно удалять только свои записи");

                        return;
                    }

                    var result = MessageBox.Show(
                        "Удалить запись?",
                        "Подтверждение",
                        MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        _context.IncidentLogs.Remove(log);

                        _context.SaveChanges();

                        LoadLogs();
                    }
                });

                card.ContextMenuStrip = menu;
            }

            container.Controls.Add(card);
        }
    }

    private Button CreateTopButton(string text)
    {
        var btn = new Button
        {
            Text = text,

            Width = 240,

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