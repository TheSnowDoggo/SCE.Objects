namespace SCE
{
    /// <summary>
    /// A class used to represent an object.
    /// </summary>
    public class SCEObject : ICContainerHolder, ISearcheable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="cList">The intitial cList of the object.</param>
        public SCEObject(CList cList)
        {
            CContainer = new(this, cList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        public SCEObject()
            : this(new CList())
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
        public CContainer CContainer { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Name:\"{Name}\" | Pos:({Position}) | Active?:{IsActive}";
        }
    }
}
