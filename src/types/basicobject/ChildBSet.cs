namespace SCE
{
    public class ChildBSet : AliasHash<SCEBObject>
    {
        public ChildBSet(SCEBObject parent)
            : base()
        {
            Parent = parent;
        }

        public SCEBObject Parent { get; }
    }
}
