using System;
using System.Drawing;
using System.Windows.Forms;
using race_game.Graphics;

namespace race_game.Screens {
    public class MainMenuScreen : Panel {
        private readonly Action<bool> _startGameCallback;
        private Button[] _menuButtons;

        public MainMenuScreen(Action<bool> startGameCallback) {
            _startGameCallback = startGameCallback;

            InitializeUI();
            SetupButtonHandlers();
        }

        private void InitializeUI() {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Navy;

            // Центральный контейнер для выравнивания
            var centerPanel = new Panel {
                Size = new Size(400, 600), // Фиксированный размер центральной области
                Location = new Point((this.Width + 10) * 2, 0),
                BackColor = Color.Transparent
            };
            this.Controls.Add(centerPanel);

            // Заголовок (центрирован в центральной панели)
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

            // Кнопки меню (вертикально в центре)
            string[] menuItems = { "Single Player", "Multiplayer", "Exit" };
            _menuButtons = new Button[menuItems.Length];

            int buttonY = 300; // Начальная позиция Y для первой кнопки
            int buttonWidth = 300;
            int buttonHeight = 50;
            int buttonSpacing = 30;

            for (int i = 0; i < menuItems.Length; i++) {
                _menuButtons[i] = CreateMenuButton(
                    menuItems[i],
                    (centerPanel.Width - buttonWidth) / 2, // Центрирование по X
                    buttonY + i * (buttonHeight + buttonSpacing),
                    buttonWidth,
                    buttonHeight);
                centerPanel.Controls.Add(_menuButtons[i]);
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
                TabStop = true,
                Tag = Array.IndexOf(_menuButtons, text)
            };

            button.FlatAppearance.BorderSize = 0;
            button.MouseEnter += (s, e) => HighlightButton((Button)s);

            return button;
        }

        private void SetupButtonHandlers() {
            _menuButtons[0].Click += (s, e) => _startGameCallback(false);
            _menuButtons[0].Focus();
            _menuButtons[1].Click += (s, e) => _startGameCallback(true);
            _menuButtons[2].Click += (s, e) => Application.Exit();
        }

        private void HighlightButton(Button button) {
            foreach (var btn in _menuButtons) {
                // Изменение цвета текста и шрифта
                btn.ForeColor = btn == button ? Color.Yellow : Color.White;
                btn.Font = new Font("Arial", 24,
                    btn == button ? FontStyle.Bold : FontStyle.Regular);

                // Изменение фона кнопки с сохранением стиля
                btn.BackColor = btn == button ? Color.Navy : Color.Transparent;
                btn.FlatAppearance.MouseOverBackColor = Color.Navy;
                btn.FlatAppearance.MouseDownBackColor = Color.Navy;
            }
        }
    }
}