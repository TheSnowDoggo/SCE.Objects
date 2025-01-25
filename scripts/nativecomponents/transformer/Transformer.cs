namespace SCE
{
    public class Transformer : ComponentBase<IObject>, IUpdate
    {
        private const string DEFAULT_NAME = "transformer";

        private Vector2 direction;

        public Transformer(string name, double speed = 1.0)
            : base(name)
        {
            Speed = speed;
        }

        public Transformer(string name, Vector2 direction, double speed = 1.0)
            : this(name, speed)
        {
            this.direction = direction.Normalize();
        }

        public Transformer(string name, float direction, double speed = 1.0)
            : this(name, speed)
        {
            this.direction = MathUtils.AngleToVector(direction);
        }

        public Transformer(double speed = 1.0)
            : this(DEFAULT_NAME, speed)
        {
        }

        public Transformer(Vector2 direction, double speed = 1.0)
            : this(DEFAULT_NAME, direction, speed)
        {
        }

        public Transformer(float direction, double speed = 1.0)
            : this(DEFAULT_NAME, direction, speed)
        {
        }

        public Vector2 Direction
        {
            get => direction;
            set => direction = value.Normalize();
        }

        public double Speed { get; set; }

        public void Update()
        {
            Holder.Position += Direction * (float)(GameHandler.DeltaTime * Speed);
        }
    }
}
