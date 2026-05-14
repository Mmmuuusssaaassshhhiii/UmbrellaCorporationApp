using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class MessageControl : UserControl
{
    private readonly UmbrellaDbContext _context;
    private readonly Employee _currentUser;

    private FlowLayoutPanel _usersPanel = null!;

    public MessageControl(UmbrellaDbContext context, Employee currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        BuildUI();
        LoadUsers();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _usersPanel?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void BuildUI()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(25, 0, 0);

        var refreshBtn = new Button
        {
            Text = "Обновить",
            Height = 35,
            Dock = DockStyle.Top
        };

        refreshBtn.Click += (_, _) => LoadUsers();
        Controls.Add(refreshBtn);

        _usersPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(20)
        };

        Controls.Add(_usersPanel);
    }

    private void LoadUsers()
    {
        var users = _context.Employees
            .Where(x => x.Id != _currentUser.Id)
            .OrderByDescending(x => x.IsOnline)
            .ThenBy(x => x.FullName)
            .ToList();

        _usersPanel.SuspendLayout();
        _usersPanel.Controls.Clear();

        foreach (var user in users)
            _usersPanel.Controls.Add(CreateUserCard(user));

        _usersPanel.ResumeLayout();
    }

    private Control CreateUserCard(Employee employee)
    {
        var panel = new Panel
        {
            Width = 650,
            Height = 90,
            BackColor = Color.FromArgb(45, 0, 0),
            Margin = new Padding(0, 0, 0, 15),
            Cursor = Cursors.Hand
        };

        panel.Controls.Add(new Label
        {
            Text = employee.FullName,
            ForeColor = Color.White,
            Font = new Font("Exo 2", 12, FontStyle.Bold),
            Location = new Point(45, 12),
            AutoSize = true
        });

        panel.Controls.Add(new Label
        {
            Text = employee.Department,
            ForeColor = Color.Gray,
            Font = new Font("Exo 2", 9),
            Location = new Point(45, 40),
            AutoSize = true
        });

        var onlineText =
            employee.IsOnline
                ? "Online"
                : $"Был: {employee.LastSeen:dd.MM HH:mm}";

        panel.Controls.Add(new Label
        {
            Text = onlineText,
            ForeColor = employee.IsOnline ? Color.LimeGreen : Color.Gray,
            Location = new Point(45, 60),
            AutoSize = true
        });

        panel.DoubleClick += (_, _) =>
        {
            new ChatForm(_context, _currentUser, employee).Show();
        };

        return panel;
    }
}