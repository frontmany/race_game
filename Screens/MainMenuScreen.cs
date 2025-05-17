using race_game.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace race_game.Screens {
    public class MainMenuScreen : Panel {
        private readonly Action<bool>   m_start_game_callback;
        private Form                    m_main_form;
        private Button[]                m_array_menu_buttons;
        int                             m_buttons_count = 3;
        int                             m_selected_index = 0;

        public MainMenuScreen(Action<bool> startGameCallback, Form mainForm) {
            m_start_game_callback = startGameCallback;
            m_main_form = mainForm;
            m_array_menu_buttons = new Button[m_buttons_count];

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            InitializeUI();
            SetupButtonHandlers();
        }

        private void InitializeUI() {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Navy;

            var centerPanel = new Panel {
                Size = new Size(400, 600),
                Location = new Point((m_main_form.Width - 400) / 2, (m_main_form.Height - 600) / 2 - 100),
                BackColor = Color.Transparent
            };
            this.Controls.Add(centerPanel);

            var titleLabel = new Label {
                Text = "RACING GAME",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(20, 200),
                Width = centerPanel.Width,
                TextAlign = ContentAlignment.MiddleCenter
            };
            centerPanel.Controls.Add(titleLabel);

            string[] menuItemsNames = { "Single Player", "Multiplayer", "Exit" };
            int buttonY = 300;
            int buttonWidth = 300;
            int buttonHeight = 50;
            int buttonSpacing = 30;

            for (int i = 0; i < m_buttons_count; i++) {
                m_array_menu_buttons[i] = CreateMenuButton(
                   menuItemsNames[i],
                   (centerPanel.Width - buttonWidth) / 2,
                   buttonY + i * (buttonHeight + buttonSpacing),
                   buttonWidth,
                   buttonHeight);
                centerPanel.Controls.Add(m_array_menu_buttons[i]);
            }
        }


        public void HandleKeyDown(object? sender, KeyEventArgs? e) {
            switch (e.KeyCode) {
                case Keys.W:
                    m_selected_index = ((m_selected_index - 1 + m_buttons_count) % m_buttons_count);
                    m_array_menu_buttons[m_selected_index].Focus();
                    HighlightButton(m_array_menu_buttons[m_selected_index]);
                    e.Handled = true;
                    break;

                case Keys.S:
                    m_selected_index = ((m_selected_index + 1) % m_buttons_count);
                    m_array_menu_buttons[m_selected_index].Focus();
                    HighlightButton(m_array_menu_buttons[m_selected_index]);
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    m_array_menu_buttons[m_selected_index].PerformClick();
                    e.Handled = true;
                    break;
            }

        }

        public void HandleKeyUp(object? sender, KeyEventArgs? e) { }

        public void HandleKeyboardEvent(object? sender, KeyEventArgs? e) {
            
        }

        private Button CreateMenuButton(string text, int xPos, int yPos, int width, int height) {
            var button = new Button {
                Text = text,
                Font = new Font("Arial", 24),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Size = new Size(width, height),
                Location = new Point(xPos, yPos),
                TabStop = true,
                Tag = Array.IndexOf(m_array_menu_buttons, text)
            };

            button.FlatAppearance.BorderSize = 0;
            button.MouseEnter += (s, e) => {
                var btn = s as Button;
                if (btn != null) {
                    HighlightButton(btn);
                }
            };


            return button;
        }

        private void SetupButtonHandlers() {
            m_array_menu_buttons[0].Click += (s, e) => m_start_game_callback(false);
            m_array_menu_buttons[1].Click += (s, e) => m_start_game_callback(true);
            m_array_menu_buttons[2].Click += (s, e) => Application.Exit();

            m_array_menu_buttons[0].Focus();
            HighlightButton(m_array_menu_buttons[0]);
        }

        private void HighlightButton(Button button) {
            foreach (var btn in m_array_menu_buttons) {
                btn.ForeColor = btn == button ? Color.Yellow : Color.White;
                btn.Font = new Font("Arial", 24, btn == button ? FontStyle.Bold : FontStyle.Regular);
                btn.BackColor = btn == button ? Color.Navy : Color.Transparent;
                btn.FlatAppearance.MouseOverBackColor = Color.Navy;
                btn.FlatAppearance.MouseDownBackColor = Color.Navy;
            }
        }
    }
}