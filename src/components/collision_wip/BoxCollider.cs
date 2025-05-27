namespace SCE
{
    public class BoxCollider : Collider
    {
        public Vector2 Offset { get; set; }

        public Vector2 Dimensions { get; set; }

        public Anchor Anchor { get; set; }

        public Rect2D CollisionArea()
        {
            var start = AnchorUtils.DimensionFix(Anchor, Dimensions) - Dimensions + Offset;
            return new(start, start + Dimensions);
        }

        public Rect2D GlobalCollisionArea()
        {
            return CollisionArea() + Holder.GridPosition();
        }

        public override bool CollidesWith(Collider other)
        {
            if (other is BoxCollider bc)
            {
                return Rect2D.Overlaps(GlobalCollisionArea(), bc.GlobalCollisionArea());
            }

            return false;
        }
    }
}
