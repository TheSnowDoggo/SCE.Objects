namespace SCE
{
    public class Welder : ComponentBase<SCEObject>, IUpdate
    {
        public enum ObjectType
        {
            Child,
            Parent,
        }

        public Welder(string name, SCEObject? obj = null)
            : base(name)
        {
            Object = obj;
        }

        public Welder(SCEObject? obj = null)
            : this("welder", obj)
        {
        }

        public SCEObject? Object { get; set; }

        public ObjectType ObjectMode { get; set; } = ObjectType.Child;

        public Vector2Int Offset { get; set; }

        public void Update()
        {
            if (Object is not null)
            {
                if (ObjectMode == ObjectType.Child)
                    Object.LocalPosition = Holder.LocalPosition + Offset;
                else
                    Holder.LocalPosition = Object.LocalPosition + Offset;
            }
        }
    }
}
