namespace SCE
{
    /// <summary>
    /// A class used to represent an object.
    /// </summary>
    public class SCEObject : ICContainerHolder, ISearcheable
    {
        private const string DEFAULT_NAME = "object";

        private World? world = null;

        public SCEObject(string name, CGroup? components = null)
        {
            Name = name;
            Components = new(this, components);
        }

        public SCEObject(CGroup? components = null)
            : this(DEFAULT_NAME, components)
        {
        }

        /// <summary>
        /// Gets or sets the name of this instance.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets the grid position of this instance.
        /// </summary>
        public Vector2Int GridPosition { get => (Vector2Int)Position.Round(); }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <inheritdoc/>
        public CContainer Components { get; }

        public World World { get => world ?? throw new NullReferenceException("World is null"); }

        public bool HasWorld { get => world is not null; }

        public void SetWorld(World? world)
        {
            this.world = world;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Name:\"{Name}\" | Pos:({Position}) | Active?:{IsActive}";
        }
    }
}
