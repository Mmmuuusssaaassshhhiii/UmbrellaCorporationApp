using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class MutationsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

    public MutationsControl(
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

        var createBtn = CreateTopButton("ДОБАВИТЬ МУТАЦИЮ");

        createBtn.Location = new Point(10, 15);

        createBtn.Click += (s, e) =>
        {
            var form = new MutationEditorForm(
                _context,
                _currentUser);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMutations();
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

        LoadMutations();
    }

    private void LoadMutations()
    {
        container.Controls.Clear();

        var mutations = _context.Mutations
            .Include(x => x.TestSubject)
            .Include(x => x.Virus)
            .OrderByDescending(x => x.ObservedAt)
            .ToList();

        foreach (var mutation in mutations)
        {
            var card = new MutationCard(mutation);

            var menu = new ContextMenuStrip();

            menu.Items.Add("Редактировать", null, (s, e) =>
            {
                var editor = new MutationEditorForm(
                    _context,
                    _currentUser,
                    mutation);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadMutations();
                }
            });

            menu.Items.Add("Удалить", null, (s, e) =>
            {
                var result = MessageBox.Show(
                    "Удалить запись?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _context.Mutations.Remove(mutation);

                    _context.SaveChanges();

                    LoadMutations();
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