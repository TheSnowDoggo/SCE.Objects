namespace SCE
{
    public class Raycast : Collider
    {
        public float Length { get; set; }

        public Vector2 Direction { get; set; } = Vector2.Right;

        public Vector2 Offset { get; set; }
        
        public Vector2 GlobalPosition()
        {
            return GlobalPosition() + Offset;
        }

        public Vector2 GlobalEndPosition()
        {
            return GlobalPosition() + (Direction.Normalize() * Length);
        }

        public override bool CollidesWith(Collider other)
        {
            if (other is Raycast rc)
            {
                var pos = GlobalPosition();
                float m = Direction.Y / Direction.X;
                float c = MathUtils.LineSolveC(pos.X, pos.Y, m);
            }

            if (other is CircleCollider cc)
            {
                var cPos = cc.GlobalPosition();

                var pos = GlobalPosition();
                var end = GlobalEndPosition();

                if (pos.DistanceFrom(cPos) <= cc.Radius || end.DistanceFrom(cPos) <= cc.Radius)
                {
                    return true;
                }

                float m = Direction.Y / Direction.X;
                float c = MathUtils.LineSolveC(pos.X, pos.Y, m);

                foreach (var sol in MathUtils.CircleLineSolveX(cPos.X, cPos.Y, cc.Radius, m, c))
                {
                    if (sol >= pos.X && sol <= end.X)
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
