namespace SCE
{
    public class Welder : ComponentBase<SCEObject>, IUpdate
    {
        public enum ObjectType
        {
            Child,
            Parent,
        }

        public Welder(SCEObject? obj = null)
            : base()
        {
            Object = obj;
        }

        public SCEObject? Object { get; set; }

        public ObjectType ObjectMode { get; set; } = ObjectType.Child;

        public Vector2Int Offset { get; set; }

        public void Update()
        {
            if (Object is not null)
            {
                if (ObjectMode == ObjectType.Child)
                    Object.Position = Holder.Position + Offset;
                else
                    Holder.Position = Object.Position + Offset;
            }
        }
    }
}
