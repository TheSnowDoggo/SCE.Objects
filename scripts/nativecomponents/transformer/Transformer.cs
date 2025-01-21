namespace SCE
{
    public class Transformer : ComponentBase<SCEObject>
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
            this.direction = direction.Normalized;
        }

        public Transformer(string name, double direction, double speed = 1.0)
            : this(name, speed)
        {
            this.direction = new Vector2((float)Math.Sin(direction), (float)Math.Cos(direction)).Normalized;
        }

        public Transformer(double speed = 1.0)
            : this(DEFAULT_NAME, speed)
        {
        }

        public Transformer(Vector2 direction, double speed = 1.0)
            : this(DEFAULT_NAME, direction, speed)
        {
        }

        public Transformer(double direction, double speed = 1.0)
            : this(DEFAULT_NAME, direction, speed)
        {
        }

        public Vector2 Direction
        {
            get => direction;
            set => direction = value.Normalized;
        }

        public double Speed { get; set; }

        public override void Update()
        {
            Parent.Position += Direction * (float)(GameHandler.DeltaTime * Speed);
        }
    }
}
