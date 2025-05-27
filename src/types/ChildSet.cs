namespace SCE
{
    public class ChildSet : AliasHash<SCEObject>
    {
        public ChildSet(SCEObject parent)
            : base()
        {
            Parent = parent;
        }

        public SCEObject Parent { get; }

        public override bool Add(SCEObject item)
        {
            if (item == Parent)
            {
                throw new ArgumentException("Tried to add itself as a child.");
            }
            var res = base.Add(item);
            if (res)
            {
                item.InitializeChild(Parent);
            }
            return res;
        }

        public override bool Remove(SCEObject item)
        {
            var res = base.Remove(item);
            if (res)
            {
                item.InitializeChild(null);
            }
            return res;
        }
    }
}
