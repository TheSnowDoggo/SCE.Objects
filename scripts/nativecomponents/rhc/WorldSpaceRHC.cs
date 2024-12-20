namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : ComponentBase<World>
    {
        private readonly List<Camera> cameraList = new();

        private readonly Queue<Camera> cameraRenderQueue = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        public WorldSpaceRHC()
            : base()
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
            foreach (Camera camera in cameraList)
            {
                if (camera.IsActive)
                    cameraRenderQueue.Enqueue(camera);
            }
        }

        private void LoadObjects()
        {
            foreach (SCEObject obj in Parent)
            {
                if (obj.IsActive)
                    TryLoadActiveObject(obj);
            }
        }

        private void TryLoadActiveObject(SCEObject obj)
        {
            foreach (IComponent component in obj.CContainer)
            {
                if (component.IsActive && component is IRenderable renderable)
                    TryLoadActiveRenderable(renderable, obj.GridPosition);
            }
        }

        private void TryLoadActiveRenderable(IRenderable renderable, Vector2Int objectOffset)
        {
            DisplayMap dpMap = renderable.GetMap();

            Vector2Int imageAlignedPos = -AnchorUtils.AnchoredDimension(renderable.Anchor, dpMap.Dimensions) + renderable.Position + objectOffset;

            Vector2Int imageAlignedPosCorner = imageAlignedPos + dpMap.Dimensions;

            foreach (Camera camera in cameraRenderQueue)
            {
                // More efficient than creating a new Area2DInt for the image
                if (Area2DInt.Overlaps(camera.WorldPositionInt, camera.WorldPositionIntCorner, imageAlignedPos, imageAlignedPosCorner))
                {
                    RenderPackage irp = new(dpMap, renderable.Layer, imageAlignedPos);

                    camera.LoadIRP(irp);
                }
            }
        }

        private void RenderCameraQueue()
        {
            foreach (Camera camera in cameraRenderQueue)
                camera.RenderNow();
            cameraRenderQueue.Clear();
        }
    }
}
