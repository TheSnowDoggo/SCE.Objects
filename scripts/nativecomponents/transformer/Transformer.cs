namespace SCE
{
    public class Transformer : ComponentBase<SCEObject>
    {
        private Vector2 direction;

        public Transformer(double speed = 1.0)
            : base()
        {
            Speed = speed;
        }

        public Transformer(Vector2 direction, double speed = 1.0)
            : this(speed)
        {
            this.direction = direction.Normalized;
        }

        public Transformer(double direction, double speed = 1.0)
            : this(speed)
        {
            this.direction = new Vector2((float)Math.Sin(direction), (float)Math.Cos(direction)).Normalized;
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
