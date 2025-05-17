using race_game.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

static class Program {
    [STAThread]
    static void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var mainForm = new Form {
            Text = "Racing Game",
            FormBorderStyle = FormBorderStyle.None,
            WindowState = FormWindowState.Maximized,
            Bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 640, 480)
        };

        new GameEngine(mainForm);
        Application.Run(mainForm);
    }
}
