namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : ComponentBase<World>
    {
        private const string DEFAULT_NAME = "world_space";

        private readonly List<Camera> cameraList = new();

        private readonly Queue<Camera> cameraRenderQueue = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        public WorldSpaceRHC(string name = DEFAULT_NAME, IObjectCacheable? iObjectCacheable = null)
            : base(name)
        {
            IObjectCacheable = iObjectCacheable;
        }

        public WorldSpaceRHC(IObjectCacheable? iObjectCacheable)
            : this(DEFAULT_NAME, iObjectCacheable)
        {
        }

        /// <summary>
        /// Gets or sets the background color of the world space.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// Gets the camera list of the world space.
        /// </summary>
        public List<Camera> CameraList { get => cameraList; }

        public bool HasCamera { get => CameraList.Count != 0; }

        public IObjectCacheable? IObjectCacheable { get; set; }

        public override void Update()
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
            foreach (var camera in cameraList)
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
                if (obj.IsActive)
                    TryLoadActiveObject(obj);
            }
        }

        private void TryLoadActiveObject(SCEObject obj)
        {
            foreach (var component in obj.CContainer)
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
    }
}
