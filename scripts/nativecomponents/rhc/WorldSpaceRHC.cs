namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : HandlerBase<IRenderable>, ICContainerHolder, IUpdate
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

        public HashSet<Camera> Cameras { get; } = new();

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
            base.LoadObjects();
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

        protected override void OnLoad(SCEObject obj, IRenderable renderable)
        {
            var dpMap = renderable.GetMap();

            var alignedPos = -AnchorUtils.AnchoredDimension(renderable.Anchor, dpMap.Dimensions) + renderable.Offset + obj.WorldGridPosition() * new Vector2Int(2, 1);

            var alignedCorner = alignedPos + dpMap.Dimensions;

            foreach (var camera in cameraRenderQueue)
            {
                // More efficient than creating a new Area2DInt for the image
                if (Area2DInt.Overlaps(camera.WorldPositionInt, camera.WorldPositionIntCorner, alignedPos, alignedCorner))
                    camera.Load(new SpritePackage(dpMap, renderable.Layer, alignedPos));
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
