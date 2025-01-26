namespace SCE
{
    public class ChildSet : SearchHash<SCEObject>
    {
        private World? world;

        public ChildSet(SCEObject parent, World? world = null)
            : base()
        {
            Parent = parent;
            this.world = world;
        }

        public SCEObject Parent { get; }

        public World? World
        {
            get => world;
            set
            {
                world = value;
                foreach (var obj in this)
                    obj.RecursiveSetWorld(world);
            }
        }

        public override bool Add(SCEObject obj)
        {
            if (obj == Parent)
                throw new RecursiveParentException("Object tried to add itself as a child object.");
            SetObject(obj, Parent, World);
            return base.Add(obj);
        }

        public override bool Remove(SCEObject obj)
        {
            bool removed = base.Remove(obj);
            if (removed)
                ClearObject(obj);
            return removed;
        }

        public override void Clear()
        {
            foreach (var obj in this)
                ClearObject(obj);
            base.Clear();
        }

        #region SetObject
        private void SetObject(SCEObject obj, SCEObject? parent, World? world)
        {
            obj.SetParent(parent);
            obj.SetWorld(world);
        }

        private void ClearObject(SCEObject obj)
        {
            SetObject(obj, null, null);
        }
        #endregion
    }
}
