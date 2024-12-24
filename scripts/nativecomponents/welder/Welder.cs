namespace SCE
{
    public class Welder : ComponentBase<SCEObject>
    {
        public Welder(SCEObject obj)
            : base()
        {
            Object = obj;
        }

        public SCEObject Object { get; set; }

        public Vector2Int Position { get; set; }

        public override void Update()
        {
            Parent.Position = Object.Position + Position;
        }
    }
}
