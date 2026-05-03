using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;

namespace UmbrellaCorporationApp
{
    public partial class AuthForm : Form
    {
        private readonly UmbrellaDbContext _context;

        public AuthForm(UmbrellaDbContext context)
        {
            InitializeComponent();
            _context = context;
            this.Load += AuthForm_Load;
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            var count = _context.Employees.Count();
            MessageBox.Show($"Employees: {count}");
        }

        private void AuthForm_Load_1(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fullName = login.Text.Trim();
            string badgeId = password.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(badgeId))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var employee = _context.Employees
                .FirstOrDefault(e => 
                    e.FullName.ToLower() == fullName.ToLower() &&
                    e.BadgeId == badgeId);

            if (employee != null)
            {
                MessageBox.Show($"Добро пожаловать, {employee.FullName}");

                var mainForm = new MainScreen(_context);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Неверное имя или Badge ID");
            }
        }

        private void login_TextChanged(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void password_TextChanged(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}