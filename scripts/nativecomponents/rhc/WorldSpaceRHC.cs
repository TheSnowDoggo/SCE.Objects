namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : ComponentBase<World>, ICContainerHolder, IUpdate
    {
        private const string DEFAULT_NAME = "world_space";

        private readonly Queue<Camera> cameraRenderQueue = new();

        public WorldSpaceRHC(string name, CGroup? components = null)
            : base(name)
        {
            Components = new(this, components);
        }

        public WorldSpaceRHC(CGroup? components = null)
            : this(DEFAULT_NAME, components)
        {
        }

        public CContainer Components { get; }

        public SCEColor BgColor { get; set; }

        public IUpdateLimit? UpdateLimiter { get; set; }

        #region Caching
        public IRenderRule? ObjectCacheable { get; set; }
        #endregion

        #region Cameras
        public HashSet<Camera> Cameras { get; } = new();

        public bool HasCamera { get => Cameras.Count != 0; }
        #endregion

        #region Update
        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
                return;
            Components.Update();
            Render();
        }

        private void Render()
        {
            if (!Components.Contains<Camera>())
                return;

            LoadCameraQueue();

            LoadObjects();

            RenderCameraQueue();
        }

        private void LoadCameraQueue()
        {
            foreach (var component in Components)
            {
                if (component.IsActive && component is Camera camera)
                    cameraRenderQueue.Enqueue(camera);
            }
        }

        private void LoadObjects()
        {
            foreach (var obj in Holder.Objects)
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
                    TryLoadActiveRenderable(renderable, obj.WorldGridPosition());
            }
        }

        private void TryLoadActiveRenderable(IRenderable renderable, Vector2Int objectOffset)
        {
            var dpMap = renderable.GetMap();

            var imageAlignedPos = -AnchorUtils.AnchoredDimension(renderable.Anchor, dpMap.Dimensions) + renderable.Offset + objectOffset;

            var imageAlignedPosCorner = imageAlignedPos + dpMap.Dimensions;

            foreach (var camera in cameraRenderQueue)
            {
                // More efficient than creating a new Area2DInt for the image
                if (Area2DInt.Overlaps(camera.WorldPositionInt, camera.WorldPositionIntCorner, imageAlignedPos, imageAlignedPosCorner))
                    camera.Load(new SpritePackage(dpMap, renderable.Layer, imageAlignedPos));
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
