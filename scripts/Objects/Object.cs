namespace SCE
{
    /// <summary>
    /// A class used to represent an object.
    /// </summary>
    public class SCEObject : ICContainerHolder
    {
        private const bool DefaultActiveState = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="cList">The intitial cList of the object.</param>
        public SCEObject(string name, CList cList)
        {
            Name = name;

            CContainer = new(this, cList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        public SCEObject(string name)
            : this(name, new CList())
        {
        }

        /// <summary>
        /// Gets or sets the name of this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive { get; set; } = DefaultActiveState;

        /// <summary>
        /// Gets the grid position of this instance.
        /// </summary>
        public Vector2Int GridPosition { get => (Vector2Int)Position.Round(); }

        /// <inheritdoc/>
        public CContainer CContainer { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Name:\"{Name}\" | Pos:({Position}) | Active?:{IsActive}";
        }
    }
}
