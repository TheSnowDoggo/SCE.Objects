namespace SCECorePlus
{
    public class ColliderLayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColliderLayer"/> class.
        /// </summary>
        public ColliderLayer(byte layer)
        {
            Layer = layer;
        }

        /// <summary>
        /// Gets or sets the layer value of the collider layer.
        /// </summary>
        public byte Layer { get; set; }

        /// <summary>
        /// Gets the total number of collidables in the collider layer.
        /// </summary>
        public int Count { get => NotListeningColliderList.Count + ListeningColliderList.Count; }

        public List<ICollidable> NotListeningColliderList { get; set; } = new();

        public List<ICollidable> ListeningColliderList { get; set; } = new();

        public List<ICollidable> GetFullColliderList()
        {
            return ListeningColliderList.Concat(NotListeningColliderList).ToList();
        }
    }
}
