namespace SCE
{
    public class ColliderHandler : HandlerBase<ICollidable>, IUpdate
    {
        private const string DEFAULT_NAME = "collider_handler";

        private readonly List<ColliderLayer> colliderLayerList = new();

        public ColliderHandler(string name = DEFAULT_NAME)
            : base(name)
        {
            ComponentRule = (collidable) => collidable.IsListening || collidable.IsReceiving;
        }

        public IRenderRule? ObjectCacheable { get; set; }

        public IUpdateLimit? RenderLimiter { get; set; }
      
        public void Update()
        {
            if (!RenderLimiter?.OnUpdate() ?? false)
                return;
            colliderLayerList.Clear();
            LoadObjects();
            CheckLayersForCollisions();
        }

        protected override void OnLoad(SCEObject obj, ICollidable collidable)
        {
            var colliderLayer = GetColliderLayer(collidable.Layer);

            if (collidable.IsListening)
                colliderLayer.ListeningColliderList.Add(collidable);
            else
                colliderLayer.NotListeningColliderList.Add(collidable);
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
