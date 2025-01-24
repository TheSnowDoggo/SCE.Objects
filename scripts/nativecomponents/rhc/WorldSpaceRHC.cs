namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : ComponentBase<World>, IUpdate
    {
        private const string DEFAULT_NAME = "world_space";

        private readonly Queue<Camera> cameraRenderQueue = new();

        public WorldSpaceRHC(string name = DEFAULT_NAME, IObjectCacheable? iObjectCacheable = null)
            : base(name)
        {
            IObjectCacheable = iObjectCacheable;
        }

        public WorldSpaceRHC(IObjectCacheable? iObjectCacheable)
            : this(DEFAULT_NAME, iObjectCacheable)
        {
        }

        public Color BgColor { get; set; }

        #region Caching
        public IObjectCacheable? IObjectCacheable { get; set; }
        #endregion

        #region Cameras
        public HashSet<Camera> Cameras { get; } = new();

        public bool HasCamera { get => Cameras.Count != 0; }
        #endregion

        #region Update
        public void Update()
        {
            Render();
        }

        private void Render()
        {
            LoadCameraQueue();

            LoadObjects();

            RenderCameraQueue();
        }

        private void LoadCameraQueue()
        {
            foreach (var camera in Cameras)
            {
                if (camera.IsActive)
                    cameraRenderQueue.Enqueue(camera);
            }
        }

        private void LoadObjects()
        {
            IEnumerable<SCEObject> collection = IObjectCacheable is null ? Parent : IObjectCacheable.ObjectCache;
            foreach (SCEObject obj in collection)
            {
                if (obj.IsActive && obj.Components.Contains<IRenderable>())
                    TryLoadActiveObject(obj);
            }
        }

        private void TryLoadActiveObject(SCEObject obj)
        {
            foreach (var component in obj.Components)
            {
                if (component.IsActive && component is IRenderable renderable)
                    TryLoadActiveRenderable(renderable, obj.GridPosition);
            }
        }

        private void TryLoadActiveRenderable(IRenderable renderable, Vector2Int objectOffset)
        {
            var dpMap = renderable.GetMap();

            var imageAlignedPos = -AnchorUtils.AnchoredDimension(renderable.Anchor, dpMap.Dimensions) + renderable.Position + objectOffset;

            var imageAlignedPosCorner = imageAlignedPos + dpMap.Dimensions;

            foreach (var camera in cameraRenderQueue)
            {
                // More efficient than creating a new Area2DInt for the image
                if (Area2DInt.Overlaps(camera.WorldPositionInt, camera.WorldPositionIntCorner, imageAlignedPos, imageAlignedPosCorner))
                    camera.LoadIRP(new SpritePackage(dpMap, renderable.Layer, imageAlignedPos));
            }
        }

        private void RenderCameraQueue()
        {
            foreach (var camera in cameraRenderQueue)
                camera.RenderNow();
            cameraRenderQueue.Clear();
        }
        #endregion
    }
}
