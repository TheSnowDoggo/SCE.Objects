namespace SCE
{
    public class CircleCollider : Collider
    {
        public Vector2 Offset { get; set; }

        public float Radius { get; set; }

        public Vector2 GlobalPosition()
        {
            return Holder.GlobalPosition + Offset;
        }

        /// <inheritdoc/>
        public override bool CollidesWith(Collider other)
        {
            if (other is CircleCollider cc)
            {
                return GlobalPosition().DistanceFrom(cc.GlobalPosition()) <= Radius;
            }

            if (other is BoxCollider bc)
            {
                var pos = GlobalPosition();
                var bcArea = bc.GlobalCollisionArea();
                
                // Area lies underneath or above circle
                if (bcArea.Top > pos.Y + Radius && bcArea.Bottom < pos.Y - Radius)
                {
                    return false;
                }

                // Area lies to the left or right of circle
                if (bcArea.Left < pos.X - Radius && bcArea.Right > pos.X + Radius)
                {
                    return false;
                }

                // Area is within the circle
                if (bcArea.Top <= pos.Y + Radius && bcArea.Bottom >= pos.Y - Radius && 
                    bcArea.Left >= pos.X - Radius && bcArea.Right <= pos.X + Radius)
                {
                    return true;
                }

                // Vertical check
                foreach (var sol in MathUtils.CircleSolveY(bcArea.Left, pos.X, pos.Y, Radius).Concat(
                    MathUtils.CircleSolveY(bcArea.Right, pos.X, pos.Y, Radius)))
                {
                    if (sol >= bcArea.Top && sol <= bcArea.Bottom)
                    {
                        return true;
                    }
                }

                // Horizontal check
                foreach (var sol in MathUtils.CircleSolveX(bcArea.Top, pos.X, pos.Y, Radius).Concat(
                    MathUtils.CircleSolveX(bcArea.Bottom, pos.X, pos.Y, Radius)))
                {
                    if (sol >= bcArea.Left && sol <= bcArea.Right)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }
    }
}
