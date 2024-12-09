namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : IComponent
    {
        private const bool DefaultActiveState = true;

        private readonly List<Camera> cameraList = new();

        private readonly Queue<Camera> cameraRenderQueue = new();

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        public WorldSpaceRHC(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = DefaultActiveState;

        /// <summary>
        /// Gets or sets the background color of the world space.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// Gets the camera list of the world space.
        /// </summary>
        public List<Camera> CameraList { get => cameraList; }

        public bool HasCamera { get => CameraList.Count != 0; }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private World World { get => (World)CContainer.CContainerHolder; }

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
            foreach (Camera camera in cameraList)
            {
                if (camera.IsActive)
                {
                    cameraRenderQueue.Enqueue(camera);
                }
            }
        }

        private void LoadObjects()
        {
            foreach (SCEObject obj in World)
            {
                if (obj.IsActive)
                {
                    TryLoadActiveObject(obj);
                }
            }
        }

        private void TryLoadActiveObject(SCEObject obj)
        {
            foreach (IComponent component in obj.CContainer)
            {
                if (component.IsActive && component is IRenderable renderable)
                {
                    TryLoadActiveRenderable(renderable, obj.GridPosition);
                }
            }
        }

        private void TryLoadActiveRenderable(IRenderable renderable, Vector2Int objectOffset)
        {
            Image image = renderable.GetImage();

            if (!image.IsActive)
            {
                return;
            }

            Vector2Int imageAlignedPos = image.Position + objectOffset;

            Vector2Int imageAlignedPosCorner = imageAlignedPos + image.Dimensions;

            foreach (Camera camera in cameraRenderQueue)
            {
                // More efficient than creating a new Area2DInt for the image
                if (Area2DInt.Overlaps(camera.WorldPositionInt, camera.WorldPositionIntCorner, imageAlignedPos, imageAlignedPosCorner))
                {
                    ImageRenderPackage irp = new(image, objectOffset);

                    camera.LoadIRP(irp);
                }
            }
        }

        private void RenderCameraQueue()
        {
            foreach (Camera camera in cameraRenderQueue)
            {
                camera.Render();
            }

            cameraRenderQueue.Clear();
        }
    }
}
