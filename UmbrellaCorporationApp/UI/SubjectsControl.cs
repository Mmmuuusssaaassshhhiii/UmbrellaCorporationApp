using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorp.Models.Enums;
using UmbrellaCorporationApp.Forms;

namespace UmbrellaCorporationApp.UI;

public class SubjectsControl : UserControl
{
    private readonly UmbrellaDbContext _context;
    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

    public SubjectsControl(UmbrellaDbContext context, Employee currentUser)
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

        if (_currentUser.ClearanceLevel == ClearanceLevel.Level10)
        {
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
        }

        container = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(20),
            BackColor = Color.FromArgb(30, 0, 0),
            WrapContents = true
        };

        Controls.Add(container);

        LoadSubjects();
    }

    private void LoadSubjects()
    {
        container.Controls.Clear();

        var subjects = _context.TestSubjects
            .Include(x => x.Virus)
            .OrderByDescending(x => x.AcquiredDate)
            .ToList();

        foreach (var subject in subjects)
        {
            var card = new SubjectCard(subject);

            void OpenSubject()
            {
                var viewer = new SubjectViewForm(subject);

                viewer.ShowDialog();
            }

            // ===== DOUBLE CLICK =====
            card.DoubleClick += (s, e) => OpenSubject();

            foreach (Control control in card.Controls)
            {
                control.DoubleClick += (s, e) => OpenSubject();
            }

            // ===== CONTEXT MENU =====
            if (_currentUser.ClearanceLevel == ClearanceLevel.Level10)
            {
                var menu = new ContextMenuStrip();

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
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        _context.TestSubjects.Remove(subject);

                        _context.SaveChanges();

                        LoadSubjects();
                    }
                });

                card.ContextMenuStrip = menu;

                foreach (Control control in card.Controls)
                {
                    control.ContextMenuStrip = menu;
                }
            }

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