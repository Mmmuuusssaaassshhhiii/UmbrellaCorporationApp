using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;

namespace UmbrellaCorporationApp.UI
{
    public class EmployeesControl : UserControl
    {
        private DataGridView grid;

        public EmployeesControl(UmbrellaDbContext context)
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(30, 0, 0);
            Padding = new Padding(0);

            InitializeGrid();
            LoadData(context);
        }

        private void InitializeGrid()
        {
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackgroundColor = Color.FromArgb(30, 0, 0),
                BorderStyle = BorderStyle.None,
                
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,

                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            grid.EnableHeadersVisualStyles = false;

            // ===== HEADER =====
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 0, 0);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Exo 2", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;

            // ===== ROWS =====
            grid.DefaultCellStyle.BackColor = Color.FromArgb(30, 0, 0);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(120, 0, 0);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Font = new Font("Exo 2", 10);

            grid.RowTemplate.Height = 35;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(45, 0, 0);

            grid.GridColor = Color.FromArgb(80, 0, 0);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            Controls.Add(grid);
        }

        private void LoadData(UmbrellaDbContext context)
        {
            var data = context.Employees
                .Select(e => new
                {
                    ФИО = e.FullName,
                    BadgeID = e.BadgeId,
                    Доступ = e.ClearanceLevel,
                    Отдел = e.Department,
                    Должность = e.Position
                })
                .ToList();

            grid.DataSource = data;
            
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.FillWeight = 1; // равномерное распределение
            }
            
            if (grid.Columns.Count >= 5)
            {
                grid.Columns[0].FillWeight = 2; // ФИО
                grid.Columns[3].FillWeight = 2; // Отдел
                grid.Columns[4].FillWeight = 2; // Должность
            }
        }
    }
}