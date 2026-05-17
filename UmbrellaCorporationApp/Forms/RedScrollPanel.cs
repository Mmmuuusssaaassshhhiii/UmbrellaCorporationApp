using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class RedScrollPanel : Panel
{
    public RedScrollPanel()
    {
        AutoScroll = true;
    }

    private const int WM_NCPAINT = 0x85;

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == WM_NCPAINT)
        {
            DrawScrollBar();
        }
    }

    private void DrawScrollBar()
    {
        using var g = CreateGraphics();

        Rectangle rect = ClientRectangle;

        var barColor = Color.FromArgb(120, 0, 0);
        var thumbColor = Color.FromArgb(180, 0, 0);

        if (VerticalScroll.Visible)
        {
            var bar = new Rectangle(
                rect.Width - SystemInformation.VerticalScrollBarWidth,
                0,
                SystemInformation.VerticalScrollBarWidth,
                rect.Height);

            g.FillRectangle(new SolidBrush(barColor), bar);

            var thumbHeight = 60;
            var thumbY = VerticalScroll.Value * rect.Height / (VerticalScroll.Maximum + 1);

            var thumb = new Rectangle(
                bar.X,
                thumbY,
                bar.Width,
                thumbHeight);

            g.FillRectangle(new SolidBrush(thumbColor), thumb);
        }
    }
}