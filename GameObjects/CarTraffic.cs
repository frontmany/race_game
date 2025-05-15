namespace race_game.GameObjects {
    public class CarTraffic : Car {
        public bool IsActive { get; private set; } = true;
        private readonly int roadSpeed;

        public CarTraffic(int x, int y, int roadSpeed)
            : base(x, y, 50, 100, Color.Gray, roadSpeed + 2) {
            this.roadSpeed = roadSpeed;
        }

        public override void Update() {
            Move(0, Speed);

            if (Bounds.Y > 600) {
                IsActive = false;
            }
        }

        public void Reset(int x, int y) {
            Bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
            IsActive = true;
        }
    }
}