namespace SCE
{
    public class Welder : ComponentBase<SCEObject>
    {
        public Welder(string name, SCEObject obj)
            : base(name)
        {
            Object = obj;
        }

        public Welder(SCEObject obj)
            : this("welder", obj)
        {
        }

        public SCEObject Object { get; set; }

        public Vector2Int Position { get; set; }

        public override void Update()
        {
            Parent.Position = Object.Position + Position;
        }
    }
}
