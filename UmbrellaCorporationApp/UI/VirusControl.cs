using System.Drawing;
using System.Windows.Forms;

namespace UmbrellaCorporationApp.UI;

public class VirusControl : UserControl
{
    public VirusControl()
    {
        this.Dock =  DockStyle.Fill;
        this.BackColor = Color.FromArgb(30, 0, 0);

        var label = new Label
        {
            Text = "Образцы и вирусы",
            ForeColor = Color.White,
            Font = new Font("Exo 2", 20),
            Dock = DockStyle.Top,
            Height = 60
        };
        
        Controls.Add(label);
    }
}