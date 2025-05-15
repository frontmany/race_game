namespace race_game.GameObjects {
    public class CarPlayer : Car {
        public bool IsActive { get; private set; } = true;

        public CarPlayer(int x, int y, Color color)
            : base(x, y, 50, 100, color, 5) {
        }

        public override void Update() {
            // Логика обновления игрока (если нужна)
        }

        public void Deactivate() {
            IsActive = false;
        }
    }
}