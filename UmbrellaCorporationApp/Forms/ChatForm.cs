using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ChatForm : Form
{
    private readonly UmbrellaDbContext _context;
    private readonly Employee _currentUser;
    private readonly Employee _targetUser;

    private FlowLayoutPanel _messages = null!;
    private TextBox _input = null!;

    public ChatForm(UmbrellaDbContext context, Employee currentUser, Employee targetUser)
    {
        _context = context;
        _currentUser = currentUser;
        _targetUser = targetUser;

        BuildUI();
        LoadMessages();
    }

    private void BuildUI()
    {
        Text = $"Chat — {_targetUser.FullName}";
        Width = 900;
        Height = 700;
        BackColor = Color.FromArgb(20, 0, 0);

        _messages = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(10)
        };

        Controls.Add(_messages);

        var bottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            BackColor = Color.FromArgb(40, 0, 0)
        };

        Controls.Add(bottom);

        _input = new TextBox
        {
            Width = 520,
            Location = new Point(10, 20),
            Font = new Font("Exo 2", 11)
        };

        bottom.Controls.Add(_input);

        var send = new Button
        {
            Text = "SEND",
            Width = 100,
            Height = 30,
            Location = new Point(540, 18)
        };

        send.Click += SendMessage;
        bottom.Controls.Add(send);

        var refresh = new Button
        {
            Text = "REFRESH",
            Width = 120,
            Height = 30,
            Location = new Point(650, 18)
        };

        refresh.Click += (_, _) => LoadMessages();
        bottom.Controls.Add(refresh);
    }

    private void LoadMessages()
    {
        var msgs = _context.Set<EmergencyMessage>()
            .AsNoTracking()
            .Where(x =>
                (x.SenderId == _currentUser.Id && x.ReceiverId == _targetUser.Id) ||
                (x.SenderId == _targetUser.Id && x.ReceiverId == _currentUser.Id))
            .OrderBy(x => x.SentAt)
            .ToList();

        _messages.SuspendLayout();
        _messages.Controls.Clear();

        foreach (var msg in msgs)
        {
            _messages.Controls.Add(new Label
            {
                Text = msg.Text,
                AutoSize = true,
                BackColor = msg.SenderId == _currentUser.Id
                    ? Color.DarkRed
                    : Color.DimGray,
                ForeColor = Color.White,
                Padding = new Padding(8),
                Margin = new Padding(5)
            });
        }

        _messages.ResumeLayout();

        if (_messages.Controls.Count > 0)
            _messages.ScrollControlIntoView(_messages.Controls[^1]);
    }

    private void SendMessage(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_input.Text))
            return;

        _context.Set<EmergencyMessage>().Add(new EmergencyMessage
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