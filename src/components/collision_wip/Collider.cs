namespace SCE
{
    public abstract class Collider : ComponentBase<SCEObject>
    {
        public LInfo LayerInfo { get; set; }

        public Action<CollisionDetails>? OnCollision;

        public abstract bool CollidesWith(Collider other);
    }
}
