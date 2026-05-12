using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;
using UmbrellaCorporationApp.Forms;
using UmbrellaCorporationApp.UI;

namespace UmbrellaCorporationApp.UI;

public class FilesControl : UserControl
{
    private readonly UmbrellaDbContext _context;
    private readonly Employee _currentUser;

    private FlowLayoutPanel container = null!;

    public FilesControl(
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

        var createBtn = CreateTopButton("СОЗДАТЬ ФАЙЛ");
        createBtn.Location = new Point(10, 15);

        createBtn.Click += (s, e) =>
        {
            var form = new FileEditorForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadFiles();
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

        LoadFiles();
    }

    private void LoadFiles()
    {
        container.Controls.Clear();

        var files = _context.ClassifiedFiles
            .Include(x => x.Author)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        foreach (var file in files)
        {
            var card = new FileCard(file);

            var menu = new ContextMenuStrip();

            menu.Items.Add("Открыть", null, (s, e) =>
            {
                new FileViewerForm(file).ShowDialog();
            });

            menu.Items.Add("Редактировать", null, (s, e) =>
            {
                if (file.AuthorId != _currentUser.Id)
                {
                    MessageBox.Show("Можно редактировать только свои файлы");
                    return;
                }

                var editor = new FileEditorForm();

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadFiles();
                }
            });

            menu.Items.Add("Удалить", null, (s, e) =>
            {
                if (file.AuthorId != _currentUser.Id)
                {
                    MessageBox.Show("Можно удалять только свои файлы");
                    return;
                }

                var result = MessageBox.Show(
                    "Удалить файл?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _context.ClassifiedFiles.Remove(file);
                    _context.SaveChanges();
                    LoadFiles();
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
            Width = 220,
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