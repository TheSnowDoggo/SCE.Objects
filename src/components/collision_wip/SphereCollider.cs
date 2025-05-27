namespace SCE
{
    internal class SphereCollider : Collider
    {
        public Vector2 Offset { get; set; }

        public float Radius { get; set; }

        public Vector2 GlobalPosition() { return GlobalPosition() + Offset; }

        public override bool CollidesWith(Collider other)
        {
            if (other is SphereCollider sc)
            {
                return GlobalPosition().DistanceFrom(sc.GlobalPosition()) <= Radius;
            }

            return false;
        }
    }
}
