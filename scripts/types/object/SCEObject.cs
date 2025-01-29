namespace SCE
{
    public class SCEObject : ICContainerHolder, IScene
    {
        #region Constructors
        public SCEObject(string name, CGroup? components = null)
        {
            Name = name;
            Components = new(this, components);
            Children = new(this);
        }

        public SCEObject(CGroup? components = null)
            : this(string.Empty, components)
        {
        }
        #endregion

        public string Name { get; set; }

        public ChildSet Children { get; }

        #region Active
        private bool isActive = true;

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                UpdateCombinedIsActive();
            }
        }

        public bool CombinedIsActive { get; private set; }

        internal void UpdateCombinedIsActive()
        {
            CombinedIsActive = RecursiveResolveActive() && IsActive;
            RecursiveUpdateCombinedIsActive();
        }
        #endregion

        #region Position
        private Vector2 localPosition;

        public Vector2 LocalPosition
        {
            get => localPosition;
            set
            {
                localPosition = value;
                UpdateWorldPosition();
            }
        }

        public Vector2 WorldPosition { get; private set; }

        public Vector2Int WorldGridPosition()
        {
            return (Vector2Int)WorldPosition.Round();
        }

        internal void UpdateWorldPosition()
        {
            WorldPosition = (Parent?.WorldPosition ?? Vector2.Zero) + localPosition;
            RecursiveUpdateWorldPositions();
        }
        #endregion

        #region Components
        public CContainer Components { get; }
        #endregion

        #region Children

        public void RecursiveResolveChildren(List<SCEObject> list)
        {
            if (Children.IsEmpty)
                return;
            list.AddRange(Children);
            foreach (var child in Children)
                child.RecursiveResolveChildren(list);
        }

        public List<SCEObject> RecursiveGetChildren()
        {
            var list = new List<SCEObject>();
            RecursiveResolveChildren(list);
            return list;
        }

        internal void RecursiveSetWorld(World? world)
        {
            SetWorld(world);
            foreach (var child in Children)
                child.RecursiveSetWorld(world);
        }

        private bool RecursiveResolveActive()
        {
            if (Parent is null)
                return true;
            if (!Parent.IsActive)
                return false;
            return Parent.RecursiveResolveActive();
        }

        private void RecursiveUpdateCombinedIsActive()
        {
            foreach (var child in Children)
            {
                child.CombinedIsActive = CombinedIsActive && child.IsActive;
                child.RecursiveUpdateCombinedIsActive();
            }
        }

        private void RecursiveUpdateWorldPositions()
        {
            foreach (var child in Children)
            {
                child.WorldPosition = WorldPosition + child.LocalPosition;
                child.RecursiveUpdateWorldPositions();
            }
        }

        #endregion

        #region Parent

        public SCEObject? Parent { get; private set; }

        public bool HasParent { get => Parent is not null; }

        internal void SetParent(SCEObject? parent)
        {
            Parent = parent;
        }

        #endregion

        #region Scene

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        internal void UpdateAll()
        {
            Components.Update();
            Update();
        }

        #endregion

        #region World

        private World? world = null;

        public World World { get => world ?? throw new NullReferenceException("World is null."); }

        public bool HasWorld { get => world is not null; }

        internal void SetWorld(World? world)
        {
            this.world = world;
        }

        #endregion

        public override string ToString()
        {
            return $"SCEObject(\"{Name}\", Pos:({LocalPosition}), Active?:{IsActive})";
        }
    }
}
