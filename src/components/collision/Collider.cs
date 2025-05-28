namespace SCE
{
    public abstract class Collider : ComponentBase<SCEObject>
    {
        public uint LayerData;

        public Action<CollisionDetails>? OnCollision;

        /// <summary>
        /// Returns whether a collision has occured between this and the given collider.
        /// </summary>
        public abstract bool CollidesWith(Collider other);
    }
}
