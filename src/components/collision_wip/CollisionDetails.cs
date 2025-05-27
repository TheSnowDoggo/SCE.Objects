namespace SCE
{
    public class CollisionDetails
    {
        public CollisionDetails(CollisionHandler handler, Collider collider, Collider receiver)
        {
            Handler = handler;
            Collider = collider;
            Receiver = receiver;
        }

        /// <summary>
        /// Gets the CollisionHandler which handled the collision.
        /// </summary>
        public CollisionHandler Handler { get; }

        /// <summary>
        /// Gets the other Collider that caused the collision.
        /// </summary>
        public Collider Collider { get; }

        /// <summary>
        /// Gets the Collider that received the collision.
        /// </summary>
        public Collider Receiver { get; }
    }
}
