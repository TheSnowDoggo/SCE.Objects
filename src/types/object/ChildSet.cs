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

        public World? World { get => Parent.World; }

        #region Modification

        /// <inheritdoc/>
        public override bool Add(SCEObject obj)
        {
            if (obj == Parent)
                throw new RecursiveParentException("Object tried to add itself as a child object.");
            if (!base.Add(obj))
                return false;         
            SetupObject(obj);
            return true;
        }

        /// <inheritdoc/>
        public override bool Remove(SCEObject obj)
        {
            if (!base.Remove(obj))
                return false;
            ClearObject(obj);
            return true;
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            ClearObjectRange(this);
            base.Clear();
        }

        #endregion

        #region SetObject

        private void SetupObject(SCEObject obj)
        {
            obj.SetParent(Parent);
            if (Parent.HasWorld)
            {
                obj.RecursiveSetWorld(World);       
                obj.UpdateRecursiveProperties();
                World.RecursiveAdd(obj);
            } 
        }

        private void ClearObject(SCEObject obj)
        {
            World.RecursiveRemove(obj);
            obj.SetParent(null);
            obj.RecursiveSetWorld(null);
            obj.UpdateRecursiveProperties();
        }

        private void ClearObjectRange(IEnumerable<SCEObject> collection)
        {
            foreach (var obj in collection)
                ClearObject(obj);
        }

        #endregion
    }
}
