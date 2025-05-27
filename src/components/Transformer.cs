namespace SCE
{
    public class Transformer : ComponentBase<SCEObject>, IUpdate
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
            this.direction = direction.Normalize();
        }

        public Transformer(float direction, double speed = 1.0)
            : this(speed)
        {
            this.direction = RotateUtils.AngleToVector(direction);
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
