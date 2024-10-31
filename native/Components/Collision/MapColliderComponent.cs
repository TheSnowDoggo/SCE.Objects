namespace SCECorePlus.Components.Collision
{
    using SCECore.ComponentSystem;

    using SCECorePlus.Objects;
    using SCECorePlus.Types;

    public class MapColliderComponent : IComponent, ICollidable
    {
        private const bool DefaultActiveState = true;
        private const bool DefaultListeningState = false;
        private const bool DefaultReceivingState = true;

        private const byte DefaultLayer = 0;

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapColliderComponent"/> class.
        /// </summary>
        public MapColliderComponent(Grid2D<bool> collisionGrid, byte layer, Anchor anchor, bool isActive = DefaultActiveState)
        {
            CollisionGrid = collisionGrid;
            Anchor = anchor;
            Layer = layer;

            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public MapColliderComponent(Grid2D<bool> collisionGrid, byte layer, bool isActive = DefaultActiveState)
            : this(collisionGrid, layer, new Anchor(), isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public MapColliderComponent(Grid2D<bool> collisionGrid, Anchor anchor, bool isActive = DefaultActiveState)
            : this(collisionGrid, DefaultLayer, anchor, isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxColliderComponent"/> class.
        /// </summary>
        public MapColliderComponent(Grid2D<bool> collisionGrid, bool isActive = DefaultActiveState)
            : this(collisionGrid, DefaultLayer, isActive)
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
        /// Gets or sets the collision grid to collide from.
        /// </summary>
        public Grid2D<bool> CollisionGrid { get; set; }

        /// <summary>
        /// Gets or sets the anchor of the collision grid.
        /// </summary>
        public Anchor Anchor { get; set; }

        public Area2DInt ObjectAlignedAnchoredCollisionArea { get => CollisionGrid.GridArea + Object.GridPosition; }

        private Vector2Int OffsetPosition { get => Object.GridPosition + Anchor.GetAlignedOffset(CollisionGrid.Dimensions); }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        public static Grid2D<bool> ConvertToCollisionGrid(DisplayMap displayMap, Color excludedBgColor = Color.Transparent)
        {
            Grid2D<bool> collisionGrid = new(displayMap.Dimensions);

            void CycleAction(Vector2Int pos)
            {
                if (displayMap[pos].BgColor != excludedBgColor)
                {
                    collisionGrid[pos] = true;
                }
            }

            displayMap.GenericCycle(CycleAction);

            return collisionGrid;
        }

        public static Grid2D<bool> ConvertToCollisionGrid(Image image, Color excludedBgColor = Color.Transparent)
        {
            return ConvertToCollisionGrid((DisplayMap)image, excludedBgColor);
        }

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
        public bool CollidesWith(ICollidable other)
        {
            if (other is MapColliderComponent mapColliderComp)
            {
                Area2DInt otherObjectAlignedArea = mapColliderComp.ObjectAlignedAnchoredCollisionArea;

                if (!ObjectAlignedAnchoredCollisionArea.OverlapsWith(otherObjectAlignedArea))
                {
                    return false;
                }

                Area2DInt thisOverlappingGridArea = CollisionGrid.GridArea.GetOverlap(otherObjectAlignedArea - OffsetPosition);

                bool collides = false;

                bool CycleActionWhile(Vector2Int thisPos)
                {
                    Vector2Int otherPos = thisPos - thisOverlappingGridArea.Start;

                    if (CollisionGrid[thisPos] && mapColliderComp.CollisionGrid[otherPos])
                    {
                        collides = true;
                    }

                    return !collides;
                }

                CollisionGrid.GenericCycleArea(CycleActionWhile, thisOverlappingGridArea);

                return collides;
            }

            if (other is BoxColliderComponent boxColliderComp)
            {
                Area2DInt otherObjectAlignedArea = boxColliderComp.ObjectAlignedAnchoredCollisionArea;

                if (!ObjectAlignedAnchoredCollisionArea.OverlapsWith(otherObjectAlignedArea))
                {
                    return false;
                }

                Area2DInt thisOverlappingGridArea = CollisionGrid.GridArea.GetOverlap(otherObjectAlignedArea - OffsetPosition);

                bool collides = false;

                bool CycleActionWhile(Vector2Int thisPos)
                {
                    if (CollisionGrid[thisPos])
                    {
                        collides = true;
                    }

                    return !collides;
                }

                CollisionGrid.GenericCycleArea(CycleActionWhile, thisOverlappingGridArea);

                return collides;
            }

            throw new NotImplementedException("No method found for given component.");
        }

        /// <inheritdoc/>
        public bool HasMethodFor(ICollidable other)
        {
            return other is MapColliderComponent or BoxColliderComponent;
        }
    }
}
