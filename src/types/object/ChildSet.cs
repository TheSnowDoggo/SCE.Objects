namespace SCE
{
    public class ChildSet : SearchHash<SCEObject>
    {
        private readonly List<SCEObject> _activeList = new();

        public ChildSet(SCEObject parent)
            : base()
        {
            Parent = parent;
        }

        public SCEObject Parent { get; }

        public World World { get => Parent.World; }

        public IList<SCEObject> ActiveList { get => _activeList.AsReadOnly(); }

        #region Modification
        public override bool Add(SCEObject obj)
        {
            if (obj == Parent)
                throw new RecursiveParentException("Object tried to add itself as a child object.");
            SetupObject(obj);
            return base.Add(obj);
        }

        public override bool Remove(SCEObject obj)
        {
            if (!base.Remove(obj))
                return false;
            ClearObject(obj);
            return true;
        }

        public override void Clear()
        {
            ClearObjectRange(this);
            base.Clear();
        }
        #endregion

        #region Update
        public void Start()
        {
            _activeList.Clear();
            foreach (var child in this)
            {
                if (child.IsActive)
                    child.Start();
            }
        }

        public void Update()
        {
            _activeList.Clear();
            foreach (var child in this)
            {
                if (child.IsActive)
                {
                    child.UpdateAll();
                    _activeList.Add(child);
                }
            }
        }
        #endregion

        #region SetObject
        private void SetupObject(SCEObject obj)
        {
            obj.SetParent(Parent);
            if (Parent.HasWorld)
            {
                obj.RecursiveSetWorld(World);  
                World.RecursiveAdd(obj);
                obj.UpdateCombinedIsActive();
                obj.UpdateWorldPosition();
            } 
        }

        private void ClearObject(SCEObject obj)
        {
            World.RecursiveRemove(obj);
            obj.SetParent(null);
            obj.RecursiveSetWorld(null);
            obj.UpdateCombinedIsActive();
            obj.UpdateWorldPosition();
        }

        private void ClearObjectRange(IEnumerable<SCEObject> collection)
        {
            foreach (var obj in collection)
                ClearObject(obj);
        }
        #endregion
    }
}
