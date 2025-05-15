namespace race_game.GameObjects {
    public abstract class Car {
        public Rectangle Bounds { get; protected set; }
        public Color Color { get; protected set; }
        public int Speed { get; protected set; }

        protected Car(int x, int y, int width, int height, Color color, int speed) {
            Bounds = new Rectangle(x, y, width, height);
            Color = color;
            Speed = speed;
        }

        public virtual void Move(int dx, int dy) {
            Bounds = new Rectangle(
                Bounds.X + dx,
                Bounds.Y + dy,
                Bounds.Width,
                Bounds.Height);
        }

        public abstract void Update();
    }
}