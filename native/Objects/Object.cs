namespace SCECorePlus.Objects
{
    using SCEComponents;

    /// <summary>
    /// A class used to represent an object.
    /// </summary>
    public class SCEObject : ICContainerHolder
    {
        private const bool DefaultActiveState = true;

        private string name;
        private Vector2 position;
        private bool isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="position">The position of the object.</param>
        /// <param name="cList">The intitial cList of the object.</param>
        /// <param name="isActive">The initial active state of the object.</param>
        public SCEObject(string name, Vector2 position, CList cList, bool isActive = DefaultActiveState)
        {
            this.name = name;
            this.position = position;
            this.isActive = isActive;

            CContainer = new(this, cList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="cList">The intitial cList of the object.</param>
        /// <param name="isActive">The initial active state of the object.</param>
        public SCEObject(string name, CList cList, bool isActive = DefaultActiveState)
            : this(name, Vector2.Zero, cList, isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="position">The position of the object.</param>
        /// <param name="isActive">The initial active state of the object.</param>
        public SCEObject(string name, Vector2 position, bool isActive = DefaultActiveState)
            : this(name, position, new CList(), isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="isActive">The initial active state of the object.</param>
        public SCEObject(string name, bool isActive = DefaultActiveState)
            : this(name, new CList(), isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCEObject"/> class.
        /// </summary>
        /// <param name="isActive">The initial active state of the object.</param>
        public SCEObject(bool isActive = DefaultActiveState)
            : this(string.Empty, isActive)
        {
        }

        /// <summary>
        /// Gets or sets the name of this instance.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                string lastName = name;

                if (lastName != value)
                {
                    name = value;

                    ObjectModifyEvent?.Invoke(this, new(ObjectModifyEventArgs.ModifyType.Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of this instance.
        /// </summary>
        public Vector2 Position
        {
            get => position;
            set
            {
                Vector2 lastPos = position;

                if (lastPos != value)
                {
                    position = value;

                    ObjectModifyEvent?.Invoke(this, new(ObjectModifyEventArgs.ModifyType.Position));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set
            {
                bool lastIsActive = isActive;

                if (lastIsActive != value)
                {
                    isActive = value;

                    ObjectModifyEvent?.Invoke(this, new(ObjectModifyEventArgs.ModifyType.IsActive));
                }
            }
        }

        /// <summary>
        /// Gets the grid position of this instance.
        /// </summary>
        public Vector2Int GridPosition { get => (Vector2Int)Position.Round(); }

        /// <inheritdoc/>
        public CContainer CContainer { get; }

        public event EventHandler<ObjectModifyEventArgs>? ObjectModifyEvent;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Name:\"{Name}\" | Pos:({Position}) | Active?:{IsActive}";
        }
    }
}
