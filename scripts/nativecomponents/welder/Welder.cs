namespace SCE
{
    public class Welder : ComponentBase<IObject>, IUpdate
    {
        public Welder(string name, IObject obj)
            : base(name)
        {
            Object = obj;
        }

        public Welder(IObject obj)
            : this("welder", obj)
        {
        }

        public IObject Object { get; set; }

        public Vector2Int Offset { get; set; }

        public void Update()
        {
            Holder.Position = Object.Position + Offset;
        }
    }
}
