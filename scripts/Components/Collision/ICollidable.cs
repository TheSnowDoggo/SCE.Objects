namespace SCECorePlus.Components.Collision
{
    using SCECorePlus.Objects;
    using SCECore.ComponentSystem;

    public interface ICollidable : IComponent
    {
        public delegate void CallOnCollision(ICollidable collider, ICollidable other);

        /// <summary>
        /// Gets the collision layer of the collision component.
        /// </summary>
        public byte Layer { get; }

        /// <summary>
        /// Gets a value indicating whether the collision component should check for collisions with other active colliders.
        /// </summary>
        /// <remarks>
        /// Note: Highly performance intensive; should only be kept on if necessary.
        /// </remarks>
        public bool IsListening { get; }

        /// <summary>
        /// Gets a value indiciating whether the collision component should be checked for collisions by other listening active colliders.
        /// </summary>
        public bool IsReceiving { get; }

        /// <summary>
        /// Gets the <see cref="CallOnCollision"/> delegate called when the collision component has collided with another collision component.
        /// </summary>
        public CallOnCollision? OnCollision { get; }

        /// <summary>
        /// Gets the parent object of the collision component.
        /// </summary>
        public SCEObject Object { get; }

        /// <summary>
        /// Determines whether the specified collision component collides with this collision component.
        /// </summary>
        /// <param name="collidable">The collision component to check.</param>
        /// <returns><see langword="true"/> if the specified <paramref name="collidable"/> collides with this collision component; otherwise, <see langword="false"/>.</returns>
        public bool CollidesWith(ICollidable collidable);

        /// <summary>
        /// Determines whether this collision component has a method for checking collision with the specified collision component type.
        /// </summary>
        /// <param name="collidable">The collision component to check.</param>
        /// <returns><see langword="true"/> if this collision component has a method for checking collision with the specified collision component type; otherwise, <see langword="false"/>.</returns>
        public bool HasMethodFor(ICollidable collidable);
    }
}
