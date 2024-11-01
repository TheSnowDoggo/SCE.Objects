namespace SCECorePlus.Components.Collision
{
    using SCEComponents;

    using SCECorePlus.Objects;

    public class ColliderHandlerComponent : IComponent
    {
        private const bool DefaultActiveState = true;

        private readonly List<ColliderLayer> colliderLayerList = new();

        private CContainer? cContainer;

        public ColliderHandlerComponent(string name, bool isActive = DefaultActiveState)
        {
            Name = name;
            IsActive = isActive;
        }

        public string Name { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        public event EventHandler? ComponentModifyEvent;

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private World World { get => (World)CContainer.CContainerHolder; }

        private List<SCEObject> ObjectList { get => World.ObjectList; }

        /// <inheritdoc/>
        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is World)
            {
                this.cContainer = cContainer;
            }
            else
            {
                throw new InvalidCContainerHolderException("CContainerHolder is not World.");
            }
        }

        /// <inheritdoc/>
        public void Update()
        {
            UpdateColliderLayerList();

            CheckLayersForCollisions();
        }

        private static bool DoCollidersCollide(ICollidable collider, ICollidable other)
        {
            if (collider.HasMethodFor(other))
            {
                return collider.CollidesWith(other);
            }
            
            if (other.HasMethodFor(collider))
            {
                return other.CollidesWith(collider);
            }

            return false;
        }

        private static void CheckForCollisionsIn(List<ICollidable> fullColliderList, ICollidable collider, int index)
        {
            int i = index + 1;

            bool layersMatch;
            do
            {
                ICollidable other = fullColliderList[i];

                layersMatch = collider.Layer == other.Layer;

                if (layersMatch && other.IsReceiving && DoCollidersCollide(collider, other))
                {
                    collider.OnCollision?.Invoke(collider, other);

                    if (other.IsListening)
                    {
                        other.OnCollision?.Invoke(other, collider);
                    }
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
                    {
                        CheckForCollisionsIn(fullColliderList, collider, i);
                    }
                }
            }
        }

        private void UpdateColliderLayerList()
        {
            colliderLayerList.Clear();

            foreach (SCEObject obj in ObjectList)
            {
                if (obj.IsActive)
                {
                    TryAddColliderComponents(obj);
                }
            }
        }

        private void TryAddColliderComponents(SCEObject obj)
        {
            foreach(IComponent component in obj.CContainer)
            {
                if (component.IsActive && component is ICollidable collidable && (collidable.IsListening || collidable.IsReceiving))
                {
                    AddCollider(collidable);
                }
            }
        }

        private void AddCollider(ICollidable collidable)
        {
            ColliderLayer colliderLayer = GetColliderLayer(collidable.Layer);

            if (collidable.IsListening)
            {
                colliderLayer.ListeningColliderList.Add(collidable);
            }
            else
            {
                colliderLayer.NotListeningColliderList.Add(collidable);
            }
        }

        private ColliderLayer GetColliderLayer(byte layer)
        {
            ColliderLayer colliderLayer;

            if (!HasColliderLayer(layer, out int index))
            {
                colliderLayer = new(layer);

                colliderLayerList.Add(colliderLayer);
            }
            else
            {
                colliderLayer = colliderLayerList[index];
            }

            return colliderLayer;
        }

        private bool HasColliderLayer(byte layer, out int index)
        {
            int i = 0;
            foreach (ColliderLayer colliderLayer in colliderLayerList)
            {
                if (colliderLayer.Layer == layer)
                {
                    index = i;
                    return true;
                }
                i++;
            }

            index = -1;
            return false;
        }
    }

}
