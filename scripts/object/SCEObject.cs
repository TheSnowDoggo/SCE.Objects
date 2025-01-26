namespace SCE
{
    public class SCEObject : ICContainerHolder, IScene
    {
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

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public ChildSet Children { get; }

        #region Position
        public Vector2 Position { get; set; }

        public Vector2 WorldPosition { get; private set; }

        public Vector2Int WorldGridPosition()
        {
            return (Vector2Int)WorldPosition.Round();
        }
        #endregion

        #region Components
        public CContainer Components { get; }
        #endregion

        #region Children
        internal void WorldStart()
        {
            RecursiveStartChildren();
        }

        internal void WorldUpdate(SearchHash<SCEObject> searchHash)
        {
            WorldPosition = Position;
            RecursiveUpdateChildren(searchHash);
        }

        public List<SCEObject> RecursiveGetChildren()
        {
            var descendents = new List<SCEObject>(Children);
            foreach (var obj in Children)
                descendents.AddRange(obj.RecursiveGetChildren());
            return descendents;
        }

        private void RecursiveStartChildren()
        {
            Start();
            foreach (var child in Children)
            {
                if (child.IsActive)
                    child.RecursiveStartChildren();
            }
        }

        private void RecursiveUpdateChildren(SearchHash<SCEObject> searchHash)
        {
            Components.Update();
            Update();
            searchHash.Add(this);
            foreach (var child in Children)
            {
                if (child.IsActive)
                {
                    child.WorldPosition = WorldPosition + child.Position;
                    child.RecursiveUpdateChildren(searchHash);
                }
            }
        }

        internal void RecursiveSetWorld(World? world)
        {
            SetWorld(world);
            foreach (var child in Children)
                child.RecursiveSetWorld(world);
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
            return $"SCEObject(\"{Name}\", Pos:({Position}), Active?:{IsActive})";
        }
    }
}
