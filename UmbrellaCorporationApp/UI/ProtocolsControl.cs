using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class ProtocolsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

    public ProtocolsControl(
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

        var createBtn = CreateTopButton("СОЗДАТЬ ПРОТОКОЛ");

        createBtn.Location = new Point(10, 15);

        createBtn.Click += (s, e) =>
        {
            var form = new ProtocolEditorForm(
                _context,
                _currentUser);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadProtocols();
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

        LoadProtocols();
    }

    private void LoadProtocols()
    {
        container.Controls.Clear();

        var protocols = _context.EmergencyProtocols
            .OrderBy(x => x.Code)
            .ToList();

        foreach (var protocol in protocols)
        {
            var card = new ProtocolCard(protocol);

            var menu = new ContextMenuStrip();

            menu.Items.Add("Редактировать", null, (s, e) =>
            {
                var editor = new ProtocolEditorForm(
                    _context,
                    _currentUser,
                    protocol);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadProtocols();
                }
            });

            menu.Items.Add("Удалить", null, (s, e) =>
            {
                var result = MessageBox.Show(
                    "Удалить протокол?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _context.EmergencyProtocols.Remove(protocol);

                    _context.SaveChanges();

                    LoadProtocols();
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

            Width = 250,

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