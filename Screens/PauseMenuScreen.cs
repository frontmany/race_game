using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace race_game.Screens {
    public class PauseMenuScreen : Panel {
        private readonly Action m_resume_callback;
        private readonly Action m_return_to_menu_callback;
        private Form m_main_form;
        private Button[] m_array_menu_buttons;
        private int m_buttons_count = 3;
        private int m_selected_index = 0;

        public PauseMenuScreen(Action resumeCallback, Action returnToMenuCallback, Form mainForm) {
            m_resume_callback = resumeCallback;
            m_return_to_menu_callback = returnToMenuCallback;
            m_main_form = mainForm;
            m_array_menu_buttons = new Button[m_buttons_count];

            InitializeUI();
            SetupButtonHandlers();
        }

        private void InitializeUI() {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(150, 0, 0, 128);

            var centerPanel = new Panel {
                Size = new Size(400, 500),
                Location = new Point((m_main_form.Width - 400) / 2, (m_main_form.Height - 500) / 2),
                BackColor = Color.Navy
            };
            this.Controls.Add(centerPanel);

            var titleLabel = new Label {
                Text = "PAUSED",
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(100, 100),
                Width = centerPanel.Width,
                TextAlign = ContentAlignment.MiddleCenter
            };
            centerPanel.Controls.Add(titleLabel);

            string[] menuItemsNames = { "Resume", "Main Menu", "Exit" };
            int buttonY = 200;
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

        private Button CreateMenuButton(string text, int xPos, int yPos, int width, int height) {
            var button = new Button {
                Text = text,
                Font = new Font("Arial", 24),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Size = new Size(width, height),
                Location = new Point(xPos, yPos),
                TabStop = true
            };

            button.FlatAppearance.BorderSize = 0;
            button.MouseEnter += (s, e) => {
                var btn = s as Button;
                if (btn != null) {
                    HighlightButton(btn);
                    m_selected_index = Array.IndexOf(m_array_menu_buttons, btn);
                }
            };

            return button;
        }

        private void SetupButtonHandlers() {
            m_array_menu_buttons[0].Click += (s, e) => m_resume_callback();
            m_array_menu_buttons[1].Click += (s, e) => m_return_to_menu_callback();
            m_array_menu_buttons[2].Click += (s, e) => Application.Exit();

            m_array_menu_buttons[0].Focus();
            HighlightButton(m_array_menu_buttons[0]);
        }

        private void HighlightButton(Button button) {
            foreach (var btn in m_array_menu_buttons) {
                btn.ForeColor = btn == button ? Color.Yellow : Color.White;
                btn.Font = new Font("Arial", 24, btn == button ? FontStyle.Bold : FontStyle.Regular);
                btn.BackColor = btn == button ? Color.FromArgb(50, 0, 0, 255) : Color.Transparent;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 0, 0, 255);
            }
        }

        public void HandleKeyDown(object? sender, KeyEventArgs? e) {
            switch (e.KeyCode) {
                case Keys.W:
                case Keys.Up:
                    m_selected_index = ((m_selected_index - 1 + m_buttons_count) % m_buttons_count);
                    m_array_menu_buttons[m_selected_index].Focus();
                    HighlightButton(m_array_menu_buttons[m_selected_index]);
                    e.Handled = true;
                    break;

                case Keys.S:
                case Keys.Down:
                    m_selected_index = ((m_selected_index + 1) % m_buttons_count);
                    m_array_menu_buttons[m_selected_index].Focus();
                    HighlightButton(m_array_menu_buttons[m_selected_index]);
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    m_array_menu_buttons[m_selected_index].PerformClick();
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    m_resume_callback();
                    e.Handled = true;
                    break;
            }
        }

        public void HandleKeyUp(object? sender, KeyEventArgs? e) { }
    }
}