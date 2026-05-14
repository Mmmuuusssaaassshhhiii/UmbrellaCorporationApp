using System.Drawing;
using System.Windows.Forms;
using UmbrellaCorp.Models;

namespace UmbrellaCorporationApp.UI;

public class SubjectCard : Panel
{
    public SubjectCard(TestSubject subject)
    {
        Width = 420;

        Height = 180;

        Margin = new Padding(15);

        BackColor = Color.FromArgb(45, 0, 0);

        BorderStyle = BorderStyle.FixedSingle;

        Cursor = Cursors.Hand;

        InitializeUI(subject);
    }

    private void InitializeUI(TestSubject subject)
    {
        var code = new Label
        {
            Text = subject.Code,

            Font = new Font("Exo 2", 16, FontStyle.Bold),

            ForeColor = Color.White,

            AutoSize = true,

            Location = new Point(20, 20)
        };

        Controls.Add(code);

        var virus = CreateInfo(
            $"Вирус: {subject.VirusId}",
            65);

        Controls.Add(virus);
        
        var status = CreateInfo(
            $"Статус: {subject.Status}",
            95);

        Controls.Add(status);

        var location = CreateInfo(
            $"Местонахождение: {subject.Location}",
            95);
        
        Controls.Add(location);
        
        var date = CreateInfo(
            $"Дата появления: {subject.AcquiredDate}",
            95);
        
        Controls.Add(date);
        
        var notes = CreateInfo(
            $"Заметки: {subject.Notes}",
            95);
        
        Controls.Add(date);
    }

    private Label CreateInfo(
        string text,
        int top)
    {
        return new Label
        {
            Text = text,

            Font = new Font("Exo2", 11),

            ForeColor = Color.Gainsboro,

            AutoSize = true,

            Location = new Point(22, top)
        };
    }
}