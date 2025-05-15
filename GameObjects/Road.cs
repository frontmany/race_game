namespace race_game.GameObjects {
    public class Road {
        public int Y { get; set; }
        public int X { get; set; }
        public int Width { get; } = 300;
        public int Height { get; } = 600;
        public int Speed { get; set; } = 5;

        public Road(int x, int y) {
            X = x;
            Y = y;
        }

        public void Update() {
            Y += Speed;
            if (Y >= Height) Y = -Height;
        }
    }
}