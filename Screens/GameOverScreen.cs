using System;
using System.Drawing;
using System.Windows.Forms;

namespace race_game.Screens {
    public class GameOverScreen : Panel {
        public bool IsMultiplayer { get; set; }
        public int FirstPlayerScore { get; set; }
        public int CrashedPlayerNumber { get; set; }
        public int SecondPlayerScore { get; set; }

        private readonly Action m_return_to_menu_callback;
        private readonly Form m_main_form;

        public GameOverScreen(Action returnToMenuCallback, Form mainForm) {
            m_return_to_menu_callback = returnToMenuCallback;
            m_main_form = mainForm;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(220, 0, 0, 0);

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        public void InitializeUI() {
            this.Controls.Clear();

            var centerPanel = new Panel {
                Size = new Size(600, 400),
                Location = new Point(
                    (m_main_form.ClientSize.Width - 600) / 2,
                    (m_main_form.ClientSize.Height - 400) / 2),
                BackColor = Color.FromArgb(240, 30, 30, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(centerPanel);

            var titleLabel = new Label {
                Text = "GAME OVER",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = false,
                Size = new Size(centerPanel.Width, 70),
                Location = new Point(0, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            centerPanel.Controls.Add(titleLabel);

            int yPos = 120;
            Font scoreFont = new Font("Arial", 24, FontStyle.Regular);

            if (IsMultiplayer) {
                string winnerText;
                Color winnerColor;

                if (CrashedPlayerNumber == 2) {
                    winnerText = "PLAYER 1 WINS!";
                    winnerColor = Color.Red;
                }
                else if (CrashedPlayerNumber == 1) {
                    winnerText = "PLAYER 2 WINS!";
                    winnerColor = Color.Blue;
                }
                else {
                    winnerText = "DRAW!";
                    winnerColor = Color.White;
                }

                var winnerLabel = new Label {
                    Text = winnerText,
                    Font = new Font("Arial", 28, FontStyle.Bold),
                    ForeColor = winnerColor,
                    AutoSize = false,
                    Size = new Size(centerPanel.Width, 50),
                    Location = new Point(0, yPos),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                centerPanel.Controls.Add(winnerLabel);
                yPos += 70;

                AddCenteredLabel(centerPanel, $"PLAYER 1: {FirstPlayerScore}", Color.White, yPos);
                yPos += 50;
                AddCenteredLabel(centerPanel, $"PLAYER 2: {SecondPlayerScore}", Color.White, yPos);
                yPos += 80;
            }
            else {
                AddCenteredLabel(centerPanel, $"YOUR SCORE: {FirstPlayerScore}", Color.Lime, yPos);
                yPos += 120;
            }

            var menuButton = new Button {
                Text = "MAIN MENU",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(300, 50),
                Location = new Point((centerPanel.Width - 300) / 2, yPos),
                Cursor = Cursors.Hand,
                TabStop = true
            };

            menuButton.FlatAppearance.BorderSize = 2;
            menuButton.FlatAppearance.BorderColor = Color.White;
            menuButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 50, 50, 50);
            menuButton.Click += (s, e) => m_return_to_menu_callback();

            centerPanel.Controls.Add(menuButton);
        }

        private void AddCenteredLabel(Panel parent, string text, Color color, int yPos) {
            var label = new Label {
                Text = text,
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = color,
                AutoSize = false,
                Size = new Size(parent.Width, 40),
                Location = new Point(0, yPos),
                TextAlign = ContentAlignment.MiddleCenter
            };
            parent.Controls.Add(label);
        }

        public void HandleKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) {
                m_return_to_menu_callback();
                e.Handled = true;
            }
        }
    }
}