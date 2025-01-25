namespace SCE
{
    public class SCEObject : IObject
    {
        private World? world = null;

        public SCEObject(string name, CGroup? components = null)
        {
            Name = name;
            Components = new(this, components);
        }

        public SCEObject(CGroup? components = null)
            : this(string.Empty, components)
        {
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        #region Position
        public Vector2 Position { get; set; }
        #endregion

        #region Components
        public CContainer Components { get; }
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
        public World World { get => world ?? throw new NullReferenceException("World is null."); }

        public bool HasWorld { get => world is not null; }

        public void SetWorld(World? world)
        {
            this.world = world;
        }
        #endregion

        public override string ToString()
        {
            return $"Object(\"{Name}\", Pos:({Position}), Active?:{IsActive})";
        }
    }
}
