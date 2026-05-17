using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ChatForm : Form
{
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(
        IntPtr hWnd,
        int Msg,
        int wParam,
        int lParam);

    private readonly UmbrellaDbContext _context;
    private readonly Employee _currentUser;
    private readonly Employee _targetUser;

    private FlowLayoutPanel _messages = null!;
    private TextBox _input = null!;

    public ChatForm(
        UmbrellaDbContext context,
        Employee currentUser,
        Employee targetUser)
    {
        _context = context;
        _currentUser = currentUser;
        _targetUser = targetUser;

        InitializeUI();
        LoadMessages();
    }

    private void InitializeUI()
    {
        Size = new Size(900, 760);

        StartPosition =
            FormStartPosition.CenterScreen;

        BackColor =
            Color.FromArgb(20, 0, 0);

        FormBorderStyle =
            FormBorderStyle.None;

        Padding =
            new Padding(1);

        MaximizeBox = false;

        // ================= TOP BAR =================

        var topBar = new Panel
        {
            Dock = DockStyle.Top,

            Height = 42,

            BackColor =
                Color.FromArgb(55, 0, 0)
        };

        Controls.Add(topBar);

        topBar.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();

                SendMessage(
                    Handle,
                    0xA1,
                    0x2,
                    0);
            }
        };

        // ================= CLOSE BUTTON =================

        var closeBtn = new Button
        {
            Text = "X",

            Dock = DockStyle.Right,

            Width = 55,

            FlatStyle = FlatStyle.Flat,

            BackColor =
                Color.FromArgb(55, 0, 0),

            ForeColor = Color.White,

            Font = new Font(
                "Exo 2",
                11,
                FontStyle.Bold),

            Cursor = Cursors.Hand
        };

        closeBtn.FlatAppearance.BorderSize = 0;

        closeBtn.MouseEnter += (s, e) =>
        {
            closeBtn.BackColor = Color.DarkRed;
        };

        closeBtn.MouseLeave += (s, e) =>
        {
            closeBtn.BackColor =
                Color.FromArgb(55, 0, 0);
        };

        closeBtn.Click += (s, e) =>
        {
            Close();
        };

        topBar.Controls.Add(closeBtn);

        // ================= TITLE =================

        var title = new Label
        {
            Text = $"ЧАТ • {_targetUser.FullName.ToUpper()}",

            Font = new Font(
                "Exo 2",
                22,
                FontStyle.Bold),

            ForeColor = Color.White,

            AutoSize = true,

            Location = new Point(30, 70)
        };

        Controls.Add(title);

        // ================= CHAT AREA =================

        _messages = new FlowLayoutPanel
        {
            Location = new Point(30, 130),

            Size = new Size(820, 500),

            AutoScroll = true,

            FlowDirection =
                FlowDirection.TopDown,

            WrapContents = false,

            BackColor =
                Color.FromArgb(30, 0, 0),

            Padding = new Padding(15),

            BorderStyle =
                BorderStyle.FixedSingle
        };

        Controls.Add(_messages);

        // ================= INPUT =================

        _input = new TextBox
        {
            Location = new Point(30, 660),

            Width = 640,

            Height = 40,

            Font = new Font(
                "Exo 2",
                12),

            BackColor =
                Color.FromArgb(40, 0, 0),

            ForeColor = Color.White,

            BorderStyle =
                BorderStyle.FixedSingle
        };

        Controls.Add(_input);

        // ================= SEND BUTTON =================

        var sendBtn = new Button
        {
            Text = "ОТПРАВИТЬ",

            Width = 180,

            Height = 42,

            Location = new Point(690, 658),

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

        sendBtn.FlatAppearance.BorderSize = 0;

        sendBtn.MouseEnter += (s, e) =>
        {
            sendBtn.BackColor =
                Color.FromArgb(180, 0, 0);
        };

        sendBtn.MouseLeave += (s, e) =>
        {
            sendBtn.BackColor =
                Color.FromArgb(120, 0, 0);
        };

        sendBtn.Click += SendMessage;

        Controls.Add(sendBtn);
    }

    private void LoadMessages()
    {
        var msgs = _context.Set<EmergencyMessage>()
            .AsNoTracking()
            .Where(x =>
                (x.SenderId == _currentUser.Id &&
                 x.ReceiverId == _targetUser.Id) ||

                (x.SenderId == _targetUser.Id &&
                 x.ReceiverId == _currentUser.Id))
            .OrderBy(x => x.SentAt)
            .ToList();

        _messages.SuspendLayout();
        _messages.Controls.Clear();

        foreach (var msg in msgs)
        {
            var isMine =
                msg.SenderId == _currentUser.Id;

            var bubble = new Panel
            {
                AutoSize = true,

                MaximumSize =
                    new Size(550, 0),

                BackColor = isMine
                    ? Color.FromArgb(120, 0, 0)
                    : Color.FromArgb(60, 60, 60),

                Padding = new Padding(12),

                Margin = isMine
                    ? new Padding(220, 5, 5, 5)
                    : new Padding(5, 5, 220, 5)
            };

            var text = new Label
            {
                Text = msg.Text,

                AutoSize = true,

                MaximumSize =
                    new Size(500, 0),

                ForeColor = Color.White,

                Font = new Font(
                    "Exo 2",
                    10)
            };

            bubble.Controls.Add(text);

            _messages.Controls.Add(bubble);
        }

        _messages.ResumeLayout();

        if (_messages.Controls.Count > 0)
        {
            _messages.ScrollControlIntoView(
                _messages.Controls[^1]);
        }
    }

    private void SendMessage(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                _input.Text))
        {
            return;
        }

        _context.Set<EmergencyMessage>()
            .Add(new EmergencyMessage
            {
                SenderId = _currentUser.Id,

                ReceiverId = _targetUser.Id,

                Text = _input.Text.Trim(),

                SentAt = DateTime.Now
            });

        _context.SaveChanges();

        _input.Clear();

        LoadMessages();
    }
}