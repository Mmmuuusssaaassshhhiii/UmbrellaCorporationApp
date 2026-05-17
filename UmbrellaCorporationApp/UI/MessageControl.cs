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

    private Panel _header = null!;
    private Panel _listContainer = null!;
    private RedScrollPanel _scrollHost = null!;

    public MessageControl(UmbrellaDbContext context, Employee currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        BuildUI();
        LoadUsers();
    }

    private void BuildUI()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(18, 0, 0);

        // ================= HEADER =================
        _header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(25, 0, 0),
            Padding = new Padding(20, 10, 20, 10)
        };

        var title = new Label
        {
            Text = "ЧАТЫ",
            ForeColor = Color.White,
            Font = new Font("Exo 2", 14, FontStyle.Bold),
            Dock = DockStyle.Left,
            AutoSize = true
        };

        var refresh = new Button
        {
            Text = "↻",
            Width = 45,
            Dock = DockStyle.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(90, 0, 0),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        refresh.FlatAppearance.BorderSize = 0;
        refresh.Click += (_, _) => LoadUsers();

        _header.Controls.Add(refresh);
        _header.Controls.Add(title);

        Controls.Add(_header);

        // ================= SCROLL HOST =================
        _scrollHost = new RedScrollPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(18, 0, 0),
            AutoScroll = true,
            Padding = new Padding(20, 80, 20, 20) // 👈 фикс перекрытия
        };

        Controls.Add(_scrollHost);

        // ================= LIST CONTAINER =================
        _listContainer = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0, 10, 0, 0)
        };

        _scrollHost.Controls.Add(_listContainer);
    }

    private void LoadUsers()
    {
        var users = _context.Employees
            .Where(x => x.Id != _currentUser.Id)
            .OrderByDescending(x => x.IsOnline)
            .ThenBy(x => x.FullName)
            .ToList();

        _listContainer.SuspendLayout();
        _listContainer.Controls.Clear();

        foreach (var user in users)
        {
            var card = CreateUserCard(user);
            _listContainer.Controls.Add(card);
            _listContainer.Controls.SetChildIndex(card, 0);
        }

        _listContainer.ResumeLayout();
    }

    private Control CreateUserCard(Employee employee)
    {
        var card = new Panel
        {
            Height = 70,
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(35, 0, 0),
            Margin = new Padding(0, 0, 0, 10),
            Cursor = Cursors.Hand
        };

        // ================= AVATAR =================
        var avatar = new Label
        {
            Size = new Size(45, 45),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.White,
            Font = new Font("Exo 2", 10, FontStyle.Bold),
            BackColor = employee.IsOnline
                ? Color.FromArgb(0, 120, 0)
                : Color.FromArgb(80, 0, 0),
            Text = GetInitials(employee.FullName),
            Location = new Point(10, 12)
        };

        avatar.Paint += (s, e) =>
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, avatar.Width, avatar.Height);
            avatar.Region = new Region(path);
        };

        // ================= NAME =================
        var name = new Label
        {
            Text = employee.FullName,
            ForeColor = Color.White,
            Font = new Font("Exo 2", 11, FontStyle.Bold),
            Location = new Point(70, 12),
            AutoSize = true
        };

        // ================= STATUS =================
        var status = new Label
        {
            Text = employee.IsOnline
                ? "online"
                : $"offline • {employee.LastSeen:dd.MM HH:mm}",
            ForeColor = employee.IsOnline ? Color.LimeGreen : Color.Gray,
            Font = new Font("Exo 2", 9),
            Location = new Point(70, 38),
            AutoSize = true
        };

        card.Controls.Add(avatar);
        card.Controls.Add(name);
        card.Controls.Add(status);

        card.MouseEnter += (_, _) =>
            card.BackColor = Color.FromArgb(55, 0, 0);

        card.MouseLeave += (_, _) =>
            card.BackColor = Color.FromArgb(35, 0, 0);

        card.DoubleClick += (_, _) =>
            new ChatForm(_context, _currentUser, employee).Show();

        return card;
    }

    private string GetInitials(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Take(2).Select(p => p[0])).ToUpper();
    }
}