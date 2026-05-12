using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI
{
    public class MessageControl : UserControl
    {
        private readonly UmbrellaDbContext _context;
        private readonly Employee _currentUser;

        private FlowLayoutPanel _usersPanel = null!;
        private System.Windows.Forms.Timer _timer = null!;

        public MessageControl(
            UmbrellaDbContext context,
            Employee currentUser)
        {
            _context = context;
            _currentUser = currentUser;

            BuildUI();

            LoadUsers();

            InitTimer();
        }

        private void BuildUI()
        {
            Dock = DockStyle.Fill;

            BackColor = Color.FromArgb(25, 0, 0);

            var title = new Label
            {
                Text = "ЛИЧНЫЕ СООБЩЕНИЯ",
                Dock = DockStyle.Top,
                Height = 60,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 18, FontStyle.Bold),
                Padding = new Padding(20, 15, 0, 0)
            };

            Controls.Add(title);

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
            _usersPanel.Controls.Clear();

            var users = _context.Employees
                .Where(x => x.Id != _currentUser.Id)
                .OrderByDescending(x => x.IsOnline)
                .ThenBy(x => x.FullName)
                .ToList();

            foreach (var user in users)
            {
                _usersPanel.Controls.Add(CreateUserCard(user));
            }
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

            // ===== ONLINE DOT =====

            var statusDot = new Panel
            {
                Size = new Size(14, 14),
                Location = new Point(20, 20),
                BackColor = employee.IsOnline
                    ? Color.LimeGreen
                    : Color.DarkRed
            };

            statusDot.Paint += (s, e) =>
            {
                e.Graphics.FillEllipse(
                    new SolidBrush(statusDot.BackColor),
                    0,
                    0,
                    14,
                    14);
            };

            // ===== NAME =====

            var name = new Label
            {
                Text = employee.FullName,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 12, FontStyle.Bold),
                Location = new Point(45, 12),
                AutoSize = true
            };

            // ===== DEPARTMENT =====

            var dep = new Label
            {
                Text = employee.Department,
                ForeColor = Color.Gray,
                Font = new Font("Exo 2", 9),
                Location = new Point(45, 40),
                AutoSize = true
            };

            // ===== STATUS =====

            string statusText = employee.IsOnline
                ? "Online"
                : $"Был в сети: {employee.LastSeen:dd.MM.yyyy HH:mm}";

            var status = new Label
            {
                Text = statusText,
                ForeColor = employee.IsOnline
                    ? Color.LimeGreen
                    : Color.DarkGray,

                Font = new Font("Exo 2", 8),
                Location = new Point(45, 60),
                AutoSize = true
            };

            // ===== LAST MESSAGE =====

            var lastMessage = _context.Set<EmergencyMessage>()
                .Where(x =>
                    (x.SenderId == _currentUser.Id &&
                     x.ReceiverId == employee.Id)
                    ||
                    (x.SenderId == employee.Id &&
                     x.ReceiverId == _currentUser.Id))
                .OrderByDescending(x => x.SentAt)
                .FirstOrDefault();

            var preview = new Label
            {
                Text = lastMessage != null
                    ? lastMessage.Text
                    : "Нет сообщений",

                ForeColor = Color.Silver,
                Font = new Font("Exo 2", 8),
                Location = new Point(350, 35),
                Width = 260,
                Height = 20
            };

            panel.Controls.Add(statusDot);
            panel.Controls.Add(name);
            panel.Controls.Add(dep);
            panel.Controls.Add(status);
            panel.Controls.Add(preview);

            void OpenChat(object? s, EventArgs e)
            {
                new ChatForm(
                    _context,
                    _currentUser,
                    employee)
                    .Show();
            }

            panel.DoubleClick += OpenChat;

            name.DoubleClick += OpenChat;
            dep.DoubleClick += OpenChat;
            status.DoubleClick += OpenChat;
            preview.DoubleClick += OpenChat;

            return panel;
        }

        private void InitTimer()
        {
            _timer = new System.Windows.Forms.Timer();

            _timer.Interval = 3000;

            _timer.Tick += (_, _) =>
            {
                LoadUsers();
            };

            _timer.Start();
        }
    }
}