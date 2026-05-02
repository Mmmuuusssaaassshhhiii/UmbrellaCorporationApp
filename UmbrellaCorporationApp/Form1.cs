using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;
using UmbrellaCorp.Data;

namespace UmbrellaCorporationApp
{
    public partial class Form1 : Form
    {
        private readonly UmbrellaDbContext _context;

        public Form1(UmbrellaDbContext context)
        {
            InitializeComponent();
            _context = context;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var count = _context.Employees.Count();
            MessageBox.Show($"Employees: {count}");
        }
    }
}