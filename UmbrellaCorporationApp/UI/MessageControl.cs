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

        public MessageControl(
            UmbrellaDbContext context,
            Employee currentUser)
        {
            _context = context;
            _currentUser = currentUser;

            BuildUI();

            LoadUsers();
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
                Width = 500,
                Height = 80,
                BackColor = Color.FromArgb(45, 0, 0),
                Margin = new Padding(0, 0, 0, 15),
                Cursor = Cursors.Hand
            };

            var name = new Label
            {
                Text = employee.FullName,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var dep = new Label
            {
                Text = employee.Department,
                ForeColor = Color.Gray,
                Font = new Font("Exo 2", 9),
                Location = new Point(20, 45),
                AutoSize = true
            };

            panel.Controls.Add(name);
            panel.Controls.Add(dep);

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

            return panel;
        }
    }
}