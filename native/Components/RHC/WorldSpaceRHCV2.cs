namespace SCECorePlus.Components.RHS
{
    using SCEComponents;

    using SCECorePlus.Objects;

    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHCV2 : IComponent
    {
        private const bool DefaultActiveState = true;

        private const Color DefaultBgColor = Color.Black;

        private readonly List<CameraV2> cameraList = new();

        private readonly List<(Area2DInt, int)> cachedCameraAreaList = new();

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHCV2"/> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        /// <param name="bgColor">The base background color.</param>
        /// <param name="isActive">The intial active state.</param>
        public WorldSpaceRHCV2(string name, Color bgColor, bool isActive = DefaultActiveState)
        {
            Name = name;
            BgColor = bgColor;
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHCV2"/> class.
        /// </summary>
        /// <param name="name">The component name.</param>
        /// <param name="isActive">The intial active state.</param>
        public WorldSpaceRHCV2(string name, bool isActive = DefaultActiveState)
            : this(name, DefaultBgColor, isActive)
        {
        }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the background color of the world space.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// Gets the camera list of the world space.
        /// </summary>
        public List<CameraV2> CameraList { get => cameraList; }

        public bool HasCamera { get => CameraList.Count != 0; }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private World World { get => (World)CContainer.CContainerHolder; }

        private List<SCEObject> ObjectList { get => World.ObjectList; }

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
            LoadCachedCameraAreaList();

            RenderObjects();
        }

        private void RenderObjects()
        {
            foreach (SCEObject obj in ObjectList)
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

        private void TryLoadActiveRenderable(IRenderable renderable, Vector2Int offset)
        {
            Image image = renderable.GetImage();

            Vector2Int imageOffsetPos = image.Position + offset;

            Vector2Int imageOffsetPosCorner = imageOffsetPos + image.Dimensions;

            foreach ((Area2DInt, int) package in cachedCameraAreaList)
            {
                Area2DInt cameraArea = package.Item1;

                if (Area2DInt.Overlaps(cameraArea.Start, cameraArea.End, imageOffsetPos, imageOffsetPosCorner))
                {
                    int index = package.Item2;

                    CameraV2 camera = cameraList[index];

                    ImageRenderPackage irp = new(image, offset);

                    camera.RenderIRP(irp);
                }
            }
        }

        private void LoadCachedCameraAreaList()
        {
            int i = 0;

            foreach (CameraV2 camera in cameraList)
            {
                if (camera.IsActive)
                {
                    camera.FillBackground(BgColor);

                    cachedCameraAreaList.Add((camera.WorldAlignedArea, i));
                }

                i++;
            }
        }
    }
}
