namespace SCECorePlus.Components.Collision
{
    using SCECorePlus.Objects;
    using SCECorePlus.Types;
    using SCECore.ComponentSystem;

    public class BoxColliderComponent : IComponent, ICollidable
    {
        private const bool DefaultActiveState = true;
        private const bool DefaultListeningState = false;
        private const bool DefaultReceivingState = true;

        private const byte DefaultLayer = 0;

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public BoxColliderComponent(Vector2Int dimensions, byte layer, Anchor anchor, bool isActive = DefaultActiveState)
        {
            if (dimensions < 1)
            {
                throw new ArgumentException("Dimensions are too small.");
            }

            Dimensions = dimensions;

            Layer = layer;
            Anchor = anchor;

            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public BoxColliderComponent(Vector2Int dimensions, byte layer, bool isActive = DefaultActiveState)
            : this(dimensions, layer, new Anchor(), isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public BoxColliderComponent(Vector2Int dimensions, bool isActive = DefaultActiveState)
            : this(dimensions, DefaultLayer, isActive)
        {
        }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public bool IsListening { get; set; } = DefaultListeningState;

        /// <inheritdoc/>
        public bool IsReceiving { get; set; } = DefaultReceivingState;

        /// <inheritdoc/>
        public byte Layer { get; set; }

        /// <inheritdoc/>
        public ICollidable.CallOnCollision? OnCollision { get; set; }

        /// <inheritdoc/>
        public SCEObject Object { get => (SCEObject)CContainer.CContainerHolder; }

        /// <summary>
        /// Gets or sets the dimensions of the box collider component.
        /// </summary>
        public Vector2Int Dimensions { get; set; }

        /// <summary>
        /// Gets or sets the anchor of the box.
        /// </summary>
        public Anchor Anchor { get; set; }

        public Area2DInt ObjectAlignedAnchoredCollisionArea { get => AnchoredCollisionArea + Object.GridPosition; }

        private Area2DInt CollisionArea { get => new(Vector2Int.Zero, Dimensions); }

        private Area2DInt AnchoredCollisionArea { get => CollisionArea + Anchor.GetAlignedOffset(Dimensions); } 

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        /// <inheritdoc/>
        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is SCEObject)
            {
                this.cContainer = cContainer;
            }
            else
            {
                throw new InvalidCContainerHolderException("CContainerHolder is not Object.");
            }
        }

        /// <inheritdoc/>
        public bool HasMethodFor(ICollidable other)
        {
            return other is BoxColliderComponent;
        }

        /// <inheritdoc/>
        public bool CollidesWith(ICollidable other)
        {
            if (other is BoxColliderComponent boxColliderComp)
            {
                return CollidesWith(boxColliderComp);
            }

            throw new NotImplementedException("No method found.");
        }

        public bool CollidesWith(BoxColliderComponent other)
        {
            return other.DoesAreaOverlapWith(ObjectAlignedAnchoredCollisionArea);
        }

        public bool DoesAreaOverlapWith(Area2DInt area)
        {
            return ObjectAlignedAnchoredCollisionArea.OverlapsWith(area);
        }
    }
}
