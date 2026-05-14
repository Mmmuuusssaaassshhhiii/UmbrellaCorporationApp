using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorporationApp.Forms;

namespace UmbrellaCorporationApp.UI;

public class SubjectsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private FlowLayoutPanel container = null!;

    public SubjectsControl(UmbrellaDbContext context)
    {
        _context = context;

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

        var createBtn = CreateTopButton("ДОБАВИТЬ ИСПЫТУЕМОГО");

        createBtn.Location = new Point(10, 15);

        createBtn.Click += (s, e) =>
        {
            var form = new SubjectEditorForm(_context);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSubjects();
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

        LoadSubjects();
    }

    private void LoadSubjects()
    {
        container.Controls.Clear();

        var subjects = _context.TestSubjects
            .OrderByDescending(x => x.VirusId)
            .ToList();

        foreach (var subject in subjects)
        {
            var card = new SubjectCard(subject);

            var menu = new ContextMenuStrip();

            menu.Items.Add("Открыть", null, (s, e) =>
            {
                var viewer = new SubjectViewForm(subject);

                viewer.ShowDialog();
            });

            menu.Items.Add("Редактировать", null, (s, e) =>
            {
                var editor = new SubjectEditorForm(
                    _context,
                    subject);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadSubjects();
                }
            });

            menu.Items.Add("Удалить", null, (s, e) =>
            {
                var result = MessageBox.Show(
                    "Удалить испытуемого?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _context.TestSubjects.Remove(subject);

                    _context.SaveChanges();

                    LoadSubjects();
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