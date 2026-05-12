using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI
{
    public class ChatForm : Form
    {
        private readonly UmbrellaDbContext _context;
        private readonly Employee _currentUser;
        private readonly Employee _targetUser;

        private FlowLayoutPanel _messages = null!;
        private TextBox _input = null!;

        private System.Windows.Forms.Timer _timer = null!;

        public ChatForm(
            UmbrellaDbContext context,
            Employee currentUser,
            Employee targetUser)
        {
            _context = context;
            _currentUser = currentUser;
            _targetUser = targetUser;

            BuildUI();

            LoadMessages();

            InitTimer();
        }

        private void BuildUI()
        {
            Text = $"Чат — {_targetUser.FullName}";

            Width = 900;
            Height = 700;

            BackColor = Color.FromArgb(20, 0, 0);

            _messages = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(15)
            };

            Controls.Add(_messages);

            var bottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(40, 0, 0)
            };

            Controls.Add(bottom);

            _input = new TextBox
            {
                Width = 650,
                Height = 40,
                Location = new Point(15, 20),
                Font = new Font("Exo 2", 11),
                BackColor = Color.FromArgb(55, 0, 0),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            bottom.Controls.Add(_input);

            var send = new Button
            {
                Text = "ОТПРАВИТЬ",
                Width = 180,
                Height = 40,
                Location = new Point(680, 20),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            send.FlatAppearance.BorderSize = 0;

            send.Click += SendMessage;

            bottom.Controls.Add(send);
        }

        private void LoadMessages()
        {
            _messages.Controls.Clear();

            var msgs = _context.Set<EmergencyMessage>()
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(x =>
                    (x.SenderId == _currentUser.Id &&
                     x.ReceiverId == _targetUser.Id)
                    ||
                    (x.SenderId == _targetUser.Id &&
                     x.ReceiverId == _currentUser.Id))
                .OrderBy(x => x.SentAt)
                .ToList();

            foreach (var msg in msgs)
            {
                AddBubble(msg);
            }
        }

        private void AddBubble(EmergencyMessage msg)
        {
            bool mine = msg.SenderId == _currentUser.Id;

            var bubble = new Panel
            {
                Width = 500,
                AutoSize = true,
                Padding = new Padding(10),
                Margin = mine
                    ? new Padding(300, 5, 5, 5)
                    : new Padding(5, 5, 300, 5),

                BackColor = mine
                    ? Color.DarkRed
                    : Color.FromArgb(50, 50, 50)
            };

            var text = new Label
            {
                Text = msg.Text,
                ForeColor = Color.White,
                Font = new Font("Exo 2", 10),
                MaximumSize = new Size(450, 0),
                AutoSize = true
            };

            bubble.Controls.Add(text);

            _messages.Controls.Add(bubble);
        }

        private void SendMessage(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_input.Text))
                return;

            var msg = new EmergencyMessage
            {
                SenderId = _currentUser.Id,
                ReceiverId = _targetUser.Id,
                Text = _input.Text.Trim(),
                SentAt = DateTime.Now
            };

            _context.Set<EmergencyMessage>().Add(msg);

            _context.SaveChanges();

            _input.Clear();

            LoadMessages();
        }

        private void InitTimer()
        {
            _timer = new System.Windows.Forms.Timer();

            _timer.Interval = 1500;

            _timer.Tick += (_, _) =>
            {
                LoadMessages();
            };

            _timer.Start();
        }
    }
}