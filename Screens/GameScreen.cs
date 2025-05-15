using race_game.Graphics;
using race_game.Core;

namespace race_game.Screens {
    public class GameScreen : Panel {
        private readonly GameState m_state;
        private readonly Renderer m_renderer;
        private readonly System.Windows.Forms.Timer m_game_timer;

        public GameScreen(Action returnToMenuCallback) {
            // Конфигурация панели
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            // Инициализация зависимостей
            m_state = new GameState();
            m_renderer = new Renderer();

            // Настройка таймера игры
            m_game_timer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            m_game_timer.Tick += (s, e) => UpdateGame();

            // Обработка ввода
            this.KeyDown += (s, e) => {
                switch (e.KeyCode) {
                    case Keys.Escape:
                        returnToMenuCallback();
                        break;
                    case Keys.W:
                        m_state.Player1.Speed = 5;
                        break;
                        // ... другие клавиши управления
                }
            };
        }

        public void Initialize(bool isMultiplayer) {
            // 1. Сброс игрового состояния
            m_state.IsMultiplayer = isMultiplayer;
            m_state.Score = 0;
            m_state.Level = 1;
            m_state.GameSpeed = 1.0f;
            m_state.IsPaused = false;
            m_state.ElapsedTime = TimeSpan.Zero;
            m_state.Status = GameState.GameStatus.Racing;

            // 2. Инициализация игроков
            m_state.Player1 = new PlayerState {
                Position = new Point(400, 500),  // Центр экрана по X, низ по Y
                Health = 100,
                Speed = 0,
                Color = Color.Red
            };

            if (isMultiplayer) {
                m_state.Player2 = new PlayerState {
                    Position = new Point(600, 500),
                    Health = 100,
                    Speed = 0,
                    Color = Color.Blue
                };
            }
            else {
                m_state.Player2 = null;
            }

            // 3. Инициализация дороги
            m_state.Road = new RoadState {
                ScrollOffset = 0,
                SegmentCount = 10
            };

            // 4. Очистка и начальное заполнение трафика
            m_state.TrafficCars.Clear();
            for (int i = 0; i < 3; i++)  // Стартовые 3 машины
            {
                m_state.TrafficCars.Add(new TrafficCarState {
                    Bounds = new Rectangle(
                        new Random().Next(100, 700),  // Случайная позиция по X
                        -100 - (i * 200),             // Распределение по Y
                        50, 100),
                    Speed = 3
                });
            }

            m_game_timer.Start();
        }

        private void UpdateGame() {
            if (m_state.Status == GameState.GameStatus.Racing) {
                m_state.Update(); // Обновление позиций машин/дороги
                this.Invalidate(); // Триггер перерисовки
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            m_renderer.Render(e.Graphics, m_state);
            base.OnPaint(e);
        }
    }
}