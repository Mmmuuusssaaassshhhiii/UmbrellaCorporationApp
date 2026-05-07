using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI
{
    public class EmployeesControl : UserControl
    {
        private readonly UmbrellaDbContext _context;

        private FlowLayoutPanel container = null!;

        public EmployeesControl(UmbrellaDbContext context)
        {
            _context = context;

            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(30, 0, 0);

            InitializeUI();
            LoadEmployees();
        }

        private void InitializeUI()
        {
            container = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(30, 0, 0)
            };

            Controls.Add(container);
        }

        private void LoadEmployees()
        {
            container.Controls.Clear();

            var employees = _context.Employees.ToList();

            foreach (var emp in employees)
            {
                container.Controls.Add(new EmployeeCard(emp));
            }
        }
    }
}