namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : HandlerBase<IRenderable>, ICContainerHolder, IUpdate
    {
        private const string DEFAULT_NAME = "world_space";

        private readonly Queue<Camera> cameraRenderQueue = new();

        #region Constructors

        public WorldSpaceRHC(string name, CGroup? components = null)
            : base(name)
        {
            Components = new(this, components);
        }

        public WorldSpaceRHC(CGroup? components = null)
            : this(DEFAULT_NAME, components)
        {
        }

        #endregion

        public CContainer Components { get; }

        #region Settings

        public SCEColor BgColor { get; set; }

        public IUpdateLimit? RenderLimiter { get; set; }

        public HashSet<Camera> Cameras { get; } = new();

        #endregion

        #region Update

        public void Update()
        {
            Components.Update();
            if (RenderLimiter?.OnUpdate() ?? true)
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

        protected override void OnLoad(SCEObject obj, IRenderable renderable)
        {
            var dpMap = renderable.GetMap();

            var start = ((-AnchorUtils.AnchoredDimension(renderable.Anchor, dpMap.Dimensions) + renderable.Offset) / new Vector2Int(2, 1) + obj.WorldGridPosition()) * new Vector2Int(2, 1);
            var end = start + (dpMap.Dimensions / new Vector2Int(2, 1));

            foreach (var camera in cameraRenderQueue)
            {
                if (camera.Overlaps(start, end))
                    camera.Load(new SpritePackage(dpMap, renderable.Layer, start));
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
