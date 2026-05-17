using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class DevelopmentsControl : UserControl
{
    private readonly UmbrellaDbContext _context;

    private readonly DataGridView grid;

    public DevelopmentsControl(UmbrellaDbContext context)
    {
        _context = context;

        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(30, 0, 0);

        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(25, 0, 0)
        };

        var addBtn = CreateButton("ДОБАВИТЬ РАЗРАБОТКУ");

        addBtn.Location = new Point(15, 15);

        addBtn.Click += (s, e) =>
        {
            var form =
                new DevelopmentEditorForm(_context);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        };

        topPanel.Controls.Add(addBtn);

        grid = CreateGrid();
        grid.Dock = DockStyle.Fill; 

        Controls.Add(grid);
        Controls.Add(topPanel);

        InitializeContextMenu();

        LoadData();
    }

    private void InitializeContextMenu()
    {
        var menu = new ContextMenuStrip();

        menu.Items.Add(
            "Редактировать",
            null,
            new EventHandler((s, e) =>
            {
                if (grid.CurrentRow == null)
                    return;

                int id = Convert.ToInt32(
                    grid.CurrentRow.Cells["Id"].Value);

                var dev = _context.Developments
                    .FirstOrDefault(x => x.Id == id);

                if (dev == null)
                    return;

                var form = new DevelopmentEditorForm(
                    _context,
                    dev);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }));

        menu.Items.Add(
            "Удалить",
            null,
            new EventHandler((s, e) =>
            {
                if (grid.CurrentRow == null)
                    return;

                int id = Convert.ToInt32(
                    grid.CurrentRow.Cells["Id"].Value);

                var dev = _context.Developments
                    .FirstOrDefault(x => x.Id == id);

                if (dev == null)
                    return;

                var result = MessageBox.Show(
                    "Удалить разработку?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    _context.Developments.Remove(dev);

                    _context.SaveChanges();

                    LoadData();
                }
            }));

        grid.MouseDown += (s, e) =>
        {
            if (e.Button != MouseButtons.Right)
                return;

            var hit = grid.HitTest(e.X, e.Y);

            if (hit.RowIndex >= 0)
            {
                grid.ClearSelection();

                grid.Rows[hit.RowIndex].Selected = true;

                grid.CurrentCell =
                    grid.Rows[hit.RowIndex].Cells[1];

                menu.Show(grid, e.Location);
            }
        };
    }

    private void LoadData()
    {
        var data = _context.Developments
            .Include(x => x.Virus)
            .Include(x => x.LeadScientist)
            .Select(x => new
            {
                Id = x.Id,

                Проект = x.ProjectName,

                Вирус = x.Virus != null
                    ? x.Virus.Name
                    : "НЕТ",

                Ученый = x.LeadScientist != null
                    ? x.LeadScientist.FullName
                    : "НЕТ",

                Статус = x.Status,

                Старт = x.StartDate.ToString("dd.MM.yyyy"),

                Завершение = x.EndDate.HasValue
                    ? x.EndDate.Value.ToString("dd.MM.yyyy")
                    : "НЕТ"
            })
            .ToList();

        grid.DataSource = data;

        if (grid.Columns.Count > 0)
        {
            grid.Columns["Id"].Visible = false;
        }

        foreach (DataGridViewColumn col in grid.Columns)
        {
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.FillWeight = 1;
        }

        if (grid.Columns.Count >= 5)
        {
            grid.Columns[1].FillWeight = 2;
            grid.Columns[3].FillWeight = 2;
        }
    }

    private DataGridView CreateGrid()
    {
        var g = new DataGridView
        {
            Dock = DockStyle.Fill,

            BackgroundColor =
                Color.FromArgb(30, 0, 0),

            BorderStyle = BorderStyle.None,

            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill,

            RowHeadersVisible = false,

            AllowUserToAddRows = false,

            AllowUserToResizeRows = false,

            AllowUserToResizeColumns = false,

            SelectionMode =
                DataGridViewSelectionMode.FullRowSelect,

            MultiSelect = false
        };

        g.EnableHeadersVisualStyles = false;

        g.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(60, 0, 0);

        g.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        g.ColumnHeadersDefaultCellStyle.Font =
            new Font("Exo 2", 10, FontStyle.Bold);

        g.ColumnHeadersHeight = 40;

        g.DefaultCellStyle.BackColor =
            Color.FromArgb(30, 0, 0);

        g.DefaultCellStyle.ForeColor =
            Color.White;

        g.DefaultCellStyle.SelectionBackColor =
            Color.FromArgb(120, 0, 0);

        g.DefaultCellStyle.SelectionForeColor =
            Color.White;

        g.DefaultCellStyle.Font =
            new Font("Exo 2", 10);

        g.RowTemplate.Height = 35;

        g.AlternatingRowsDefaultCellStyle.BackColor =
            Color.FromArgb(45, 0, 0);

        g.GridColor =
            Color.FromArgb(80, 0, 0);

        return g;
    }

    private Button CreateButton(string text)
    {
        var btn = new Button
        {
            Text = text,

            Width = 250,
            
            Cursor =  Cursors.Hand,

            Height = 40,

            FlatStyle = FlatStyle.Flat,

            BackColor = Color.FromArgb(120, 0, 0),

            ForeColor = Color.White,

            Font = new Font(
                "Exo 2",
                10,
                FontStyle.Bold)
        };

        btn.FlatAppearance.BorderSize = 0;

        btn.MouseEnter += (s, e) =>
        {
            btn.BackColor =
                Color.FromArgb(180, 0, 0);
        };

        btn.MouseLeave += (s, e) =>
        {
            btn.BackColor =
                Color.FromArgb(120, 0, 0);
        };

        return btn;
    }
}