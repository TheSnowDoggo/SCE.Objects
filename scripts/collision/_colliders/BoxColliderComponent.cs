namespace SCE
{
    public class BoxColliderComponent : ComponentBase<SCEObject>, ICollidable
    {
        private const byte DefaultLayer = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public BoxColliderComponent(Vector2Int dimensions)
            : base()
        {
            if (dimensions < 1)
                throw new ArgumentException("Dimensions are too small.");
            Dimensions = dimensions;
        }

        /// <inheritdoc/>
        public bool IsListening { get; set; } = false;

        /// <inheritdoc/>
        public bool IsReceiving { get; set; } = true;

        /// <inheritdoc/>
        public byte Layer { get; set; } = DefaultLayer;

        /// <inheritdoc/>
        public ICollidable.CallOnCollision? OnCollision { get; set; }

        /// <summary>
        /// Gets or sets the dimensions of the box collider component.
        /// </summary>
        public Vector2Int Dimensions { get; set; }

        public Vector2Int Position { get; set; }

        /// <summary>
        /// Gets or sets the anchor of the box.
        /// </summary>
        public Anchor Anchor { get; set; }

        public Area2DInt ObjectAlignedAnchoredCollisionArea { get => AnchoredCollisionArea + Parent.GridPosition; }

        private Area2DInt CollisionArea { get => new(Vector2Int.Zero, Dimensions); }

        private Area2DInt AnchoredCollisionArea { get => CollisionArea + -AnchorUtils.AnchoredDimension(Anchor, Dimensions) + Position; } 

        /// <inheritdoc/>
        public bool HasMethodFor(ICollidable other)
        {
            return other is BoxColliderComponent;
        }

        /// <inheritdoc/>
        public bool CollidesWith(ICollidable other)
        {
            if (other is not BoxColliderComponent boxColliderComp)
                throw new NotImplementedException("No method found.");
            return CollidesWith(boxColliderComp);           
        }

        public bool CollidesWith(BoxColliderComponent other)
        {
            return other.DoesAreaOverlapWith(ObjectAlignedAnchoredCollisionArea);
        }

        public bool DoesAreaOverlapWith(Area2DInt area)
        {
            return Area2DInt.Overlaps(ObjectAlignedAnchoredCollisionArea, area);
        }
    }
}
