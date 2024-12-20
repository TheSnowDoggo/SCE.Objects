namespace SCE
{
    public class MapColliderComponent : ComponentBase<SCEObject>, ICollidable
    {
        private const byte DefaultLayer = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapColliderComponent"/> class.
        /// </summary>
        public MapColliderComponent(Grid2D<bool> collisionGrid)
            : base()
        {
            CollisionGrid = collisionGrid;
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
        /// Gets or sets the collision grid to collide from.
        /// </summary>
        public Grid2D<bool> CollisionGrid { get; set; }

        public Vector2Int Position { get; set; }

        /// <summary>
        /// Gets or sets the anchor of the collision grid.
        /// </summary>
        public Anchor Anchor { get; set; }

        public Area2DInt ObjectAlignedAnchoredCollisionArea { get => CollisionGrid.GridArea + Parent.GridPosition; }

        private Vector2Int OffsetPosition { get => Parent.GridPosition + -AnchorUtils.AnchoredDimension(Anchor, CollisionGrid.Dimensions) + Position; }

        public static Grid2D<bool> ConvertToCollisionGrid(DisplayMap displayMap, Color excludedBgColor = Color.Transparent)
        {
            Grid2D<bool> collisionGrid = new(displayMap.Dimensions);

            void CycleAction(Vector2Int pos)
            {
                if (displayMap[pos].BgColor != excludedBgColor)
                    collisionGrid[pos] = true;
            }

            displayMap.GenericCycle(CycleAction);

            return collisionGrid;
        }

        public static Grid2D<bool> ConvertToCollisionGrid(Image image, Color excludedBgColor = Color.Transparent)
        {
            return ConvertToCollisionGrid((DisplayMap)image, excludedBgColor);
        }

        /// <inheritdoc/>
        public bool CollidesWith(ICollidable other)
        {
            if (other is MapColliderComponent mapColliderComp)
            {
                Area2DInt otherObjectAlignedArea = mapColliderComp.ObjectAlignedAnchoredCollisionArea;

                if (!Area2DInt.Overlaps(ObjectAlignedAnchoredCollisionArea, otherObjectAlignedArea))
                    return false;

                Area2DInt thisOverlappingGridArea = Area2DInt.GetOverlap(CollisionGrid.GridArea, otherObjectAlignedArea - OffsetPosition); 

                bool collides = false;

                bool CycleActionWhile(Vector2Int thisPos)
                {
                    Vector2Int otherPos = thisPos - thisOverlappingGridArea.Start;

                    if (CollisionGrid[thisPos] && mapColliderComp.CollisionGrid[otherPos])
                        collides = true;
                    return !collides;
                }

                CollisionGrid.GenericCycleArea(CycleActionWhile, thisOverlappingGridArea);

                return collides;
            }

            if (other is BoxColliderComponent boxColliderComp)
            {
                Area2DInt otherObjectAlignedArea = boxColliderComp.ObjectAlignedAnchoredCollisionArea;

                if (!Area2DInt.Overlaps(ObjectAlignedAnchoredCollisionArea, otherObjectAlignedArea))
                    return false;

                Area2DInt thisOverlappingGridArea = Area2DInt.GetOverlap(CollisionGrid.GridArea, otherObjectAlignedArea - OffsetPosition);

                bool collides = false;

                bool CycleActionWhile(Vector2Int thisPos)
                {
                    if (CollisionGrid[thisPos])
                        collides = true;
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
