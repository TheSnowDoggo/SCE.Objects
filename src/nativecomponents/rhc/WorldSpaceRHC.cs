namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : HandlerBase<IRenderable>, IUpdate
    {
        private readonly Queue<Camera> cameraRenderQueue = new();

        public WorldSpaceRHC()
            : base()
        {
        }

        #region Settings

        public SCEColor BgColor { get; set; }

        public IUpdateLimit? RenderLimiter { get; set; }

        public HashSet<Camera> Cameras { get; } = new();

        #endregion

        #region Update

        /// <inheritdoc/>
        public void Update()
        {
            if (RenderLimiter?.OnUpdate() ?? true)
                Render();
        }

        private void Render()
        {
            if (!Holder.Components.Contains<Camera>())
                return;
            LoadCameraQueue();
            LoadObjects();
            RenderCameraQueue();
        }

        private void LoadCameraQueue()
        {
            foreach (var c in Holder.Components.EnumerateType<Camera>())
                if (c.IsActive)
                    cameraRenderQueue.Enqueue(c);
        }

        /// <inheritdoc/>
        protected override void OnLoad(SCEObject obj, IRenderable r)
        {
            var dpMap = r.GetMap();

            var start = ((r.Offset - AnchorUtils.DimensionFix(r.Anchor, dpMap.Dimensions)) 
                / new Vector2Int(2, 1) + obj.WorldGridPosition()) * new Vector2Int(2, 1);
            var end = start + dpMap.Dimensions;

            foreach (var camera in cameraRenderQueue)
                if (camera.Overlaps(start, end))
                    camera.Load(new SpritePackage(dpMap, r.Layer, start));
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
