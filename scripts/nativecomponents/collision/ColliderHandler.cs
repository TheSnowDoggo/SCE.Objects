namespace SCE
{
    public class ColliderHandler : ComponentBase<World>, IUpdate
    {
        private const string DEFAULT_NAME = "collider_handler";

        private readonly List<ColliderLayer> colliderLayerList = new();

        public ColliderHandler(string name = DEFAULT_NAME, IObjectCacheable? iObjectCacheable = null)
            : base(name)
        {
            IObjectCacheable = iObjectCacheable;
        }

        public ColliderHandler(IObjectCacheable? iObjectCacheable)
            : this(DEFAULT_NAME, iObjectCacheable)
        {
        }

        public IObjectCacheable? IObjectCacheable { get; set; }

        /// <inheritdoc/>
        public void Update()
        {
            if (!IsActive)
                return;

            UpdateColliderLayerList();

            CheckLayersForCollisions();
        }

        private static bool DoCollidersCollide(ICollidable collider, ICollidable other)
        {
            if (collider.HasMethodFor(other))
                return collider.CollidesWith(other);
            if (other.HasMethodFor(collider))
                return other.CollidesWith(collider);
            return false;
        }

        private static void CheckForCollisionsIn(List<ICollidable> fullColliderList, ICollidable collider, int index)
        {
            int i = index + 1;

            bool layersMatch;
            do
            {
                var other = fullColliderList[i];

                layersMatch = collider.Layer == other.Layer;

                if (layersMatch && other.IsReceiving && DoCollidersCollide(collider, other))
                {
                    collider.OnCollision?.Invoke(collider, other);

                    if (other.IsListening)
                        other.OnCollision?.Invoke(other, collider);
                }

                i++;
            }
            while (i < fullColliderList.Count && layersMatch);
        }

        private void CheckLayersForCollisions()
        {
            foreach (ColliderLayer layer in colliderLayerList)
            {
                List<ICollidable> fullColliderList = layer.GetFullColliderList();

                for (int i = 0; i < fullColliderList.Count - 1; i++)
                {
                    ICollidable collider = fullColliderList[i];

                    if (collider.IsListening)
                        CheckForCollisionsIn(fullColliderList, collider, i);
                }
            }
        }

        private void UpdateColliderLayerList()
        {
            colliderLayerList.Clear();

            IEnumerable<SCEObject> collection = IObjectCacheable is null ? Parent : IObjectCacheable.ObjectCache;
            foreach (SCEObject obj in collection)
            {
                if (obj.IsActive && obj.Components.Contains<IComponent>())
                    TryAddColliderComponents(obj);
            }
        }

        private void TryAddColliderComponents(SCEObject obj)
        {
            foreach(IComponent component in obj.Components)
            {
                if (component.IsActive && component is ICollidable collidable && (collidable.IsListening || collidable.IsReceiving))
                    AddCollider(collidable);
            }
        }

        private void AddCollider(ICollidable collidable)
        {
            ColliderLayer colliderLayer = GetColliderLayer(collidable.Layer);

            if (collidable.IsListening)
                colliderLayer.ListeningColliderList.Add(collidable);
            else
                colliderLayer.NotListeningColliderList.Add(collidable);
        }

        private ColliderLayer GetColliderLayer(byte layer)
        {
            ColliderLayer colliderLayer;

            if (Contains(layer, out int index))
                colliderLayer = colliderLayerList[index];
            else
            {
                colliderLayer = new(layer);

                colliderLayerList.Add(colliderLayer);
            }

            return colliderLayer;
        }

        private bool Contains(byte layer, out int index)
        {
            for (int i = 0; i < colliderLayerList.Count; ++i)
            {
                if (colliderLayerList[i].Layer == layer)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
    }

}
