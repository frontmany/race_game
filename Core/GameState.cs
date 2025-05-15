namespace race_game.Core {
    public class GameState {
        // 1. Игровые параметры
        public bool IsMultiplayer { get; set; }
        public int Score { get; set; }
        public int Level { get; set; }
        public float GameSpeed { get; set; } = 1.0f;
        public bool IsPaused { get; set; }
        public TimeSpan ElapsedTime { get; set; }

        // 2. Позиции объектов
        public PlayerState Player1 { get; set; }
        public PlayerState Player2 { get; set; }
        public List<TrafficCarState> TrafficCars { get; } = new();
        public RoadState Road { get; set; }

        // 3. Состояние игры
        public GameStatus Status { get; set; } = GameStatus.Menu;

        public enum GameStatus {
            Menu,
            Racing,
            Paused,
            GameOver,
            Results
        }

        public void Update() {
            if (Status != GameStatus.Racing || IsPaused)
                return;

            // 1. Обновление времени
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1 / 60f)); // Предполагаем 60 FPS

            // 2. Обновление дороги (эффект движения)
            Road.ScrollOffset = (Road.ScrollOffset + (int)(5 * GameSpeed)) % 100;

            // 3. Обновление игроков
            Player1.Position = new Point(
                Player1.Position.X,
                Math.Clamp(Player1.Position.Y + Player1.Speed, 0, 600));

            if (IsMultiplayer) {
                Player2.Position = new Point(
                    Player2.Position.X,
                    Math.Clamp(Player2.Position.Y + Player2.Speed, 0, 600));
            }

            // 4. Обновление трафика
            foreach (var trafficCar in TrafficCars) {
                trafficCar.Bounds = new Rectangle(
                    trafficCar.Bounds.X,
                    trafficCar.Bounds.Y + (int)(trafficCar.Speed * GameSpeed),
                    trafficCar.Bounds.Width,
                    trafficCar.Bounds.Height);
            }

            // 5. Удаление машин за пределами экрана
            TrafficCars.RemoveAll(car => car.Bounds.Y > 700);

            // 6. Генерация нового трафика (каждые 60 кадров)
            if (ElapsedTime.TotalSeconds % 1 < 1 / 60f) {
                TrafficCars.Add(new TrafficCarState {
                    Bounds = new Rectangle(new Random().Next(100, 700), -100, 50, 100),
                    Speed = 3 + Level
                });
            }

            // 7. Проверка столкновений
            CheckCollisions();

            // 8. Увеличение сложности
            if (Score % 500 == 0) {
                Level++;
                GameSpeed = Math.Min(GameSpeed + 0.1f, 2.0f);
            }

            // 9. Обновление счета
            Score += (int)(10 * GameSpeed);
        }

        private void CheckCollisions() {
            var playerRect = new Rectangle(Player1.Position.X - 25, Player1.Position.Y - 50, 50, 100);

            foreach (var trafficCar in TrafficCars) {
                if (playerRect.IntersectsWith(trafficCar.Bounds)) {
                    Player1.Health -= 10;
                    if (Player1.Health <= 0) {
                        Status = GameStatus.GameOver;
                    }
                    break;
                }
            }

            // Аналогичная проверка для второго игрока...
        }
    }

    // Вспомогательные классы для компонентов
    public class PlayerState {
        public Point Position { get; set; }
        public int Health { get; set; } = 100;
        public int Speed { get; set; } = 5;
        public Color Color { get; set; }
    }

    public class TrafficCarState {
        public Rectangle Bounds { get; set; }
        public int Speed { get; set; }
    }

    public class RoadState {
        public int ScrollOffset { get; set; }
        public int SegmentCount { get; set; }
    }
}