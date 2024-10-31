namespace SCECorePlus.Components.RHS
{
    using SCECore.ComponentSystem;

    using SCECorePlus.Objects;

    // WorldSpace RenderHandlerComponent
    public class WorldSpaceRHC : IRenderable, IComponent
    {
        private const bool DefaultActiveState = true;

        private const byte DefaultBgColor = Color.Black;

        private readonly List<ImageRenderPackage> renderList = new();

        private readonly List<Area2DInt> areaClearList = new();

        private readonly List<Camera> cameraList = new();

        private readonly List<Area2DInt> cameraWorldAlignedAreaCacheList = new();

        private CContainer? cContainer;

        private byte bgColor = DefaultBgColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="dimensions">The dimensions of the world space.</param>
        /// <param name="isActive">The initial active state of the world space.</param>
        public WorldSpaceRHC(Vector2Int dimensions, bool isActive = DefaultActiveState)
        {
            Image = new(dimensions, isActive);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="dimensions">The dimensions of the world space.</param>
        /// <param name="bgColor">The background color of the world space.</param>
        /// <param name="isActive">The intial active state of the world space.</param>
        public WorldSpaceRHC(Vector2Int dimensions, byte bgColor, bool isActive = DefaultActiveState)
        {
            Image = new(dimensions, bgColor, isActive);

            BgColor = bgColor;
        }

        /// <summary>
        /// Gets the grid area of the world space.
        /// </summary>
        public Area2DInt GridArea { get => Image.GridArea; }

        /// <summary>
        /// Gets or sets a value indicating whether the world space is active.
        /// </summary>
        public bool IsActive
        {
            get => Image.IsActive;
            set => Image.IsActive = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector2Int"/> position of the world space.
        /// </summary>
        public Vector2Int Position
        {
            get => Image.Position;
            set => Image.Position = value;
        }

        /// <summary>
        /// Gets or sets the render layer of the world space.
        /// </summary>
        public byte Layer
        {
            get => Image.Layer;
            set => Image.Layer = value;
        }

        /// <summary>
        /// Gets or sets the background color of the world space.
        /// </summary>
        public byte BgColor
        {
            get => bgColor;
            set => bgColor = Color.ValidSet(value);
        }

        /// <summary>
        /// Gets the camera list of the world space.
        /// </summary>
        public List<Camera> CameraList => cameraList;

        /// <summary>
        /// Gets a value indicating whether the world space contains any camera.
        /// </summary>
        public bool HasCamera => CameraList.Count != 0;

        /// <summary>
        /// Gets or sets a value indicating whether the world space should render when GameHandler calls OnUpdate, rather called through IRenderable.
        /// </summary>
        public bool RenderOnUpdate { get; set; } = false;

        /// <summary>
        /// Gets the current dimensions of the world space.
        /// </summary>
        public Vector2Int Dimensions => Image.Dimensions;

        /// <summary>
        /// Gets the current width of the world space.
        /// </summary>
        public int Width => Image.Width;

        /// <summary>
        /// Gets the current height of the world space.
        /// </summary>
        public int Height => Image.Height;

        private Area2DInt CachedGridArea { get; set; }

        private Image Image { get; }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private World World => (World)CContainer.CContainerHolder;

        private List<SCEObject> ObjectList => World.ObjectList;

        /// <inheritdoc/>
        public Image GetImage()
        {
            if (!RenderOnUpdate)
            {
                Render();
            }

            return Image;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Update()
        {
            if (RenderOnUpdate)
            {
                Render();
            }
        }

        /// <summary>
        /// Resizes the world space clearing all previous data.
        /// </summary>
        /// <param name="width">The new width of the world space.</param>
        /// <param name="height">The new height of the world space.</param>
        public void CleanResize(int width, int height)
        {
            Image.CleanResize(width, height);
        }

        /// <summary>
        /// Resizes the world space clearing all previous data.
        /// </summary>
        /// <param name="dimensions">The new dimensions of the world space.</param>
        public void CleanResize(Vector2Int dimensions)
        {
            Image.CleanResize(dimensions);
        }

        private void SmartClear()
        {
            Pixel clearPixel = new(BgColor);

            foreach (Area2DInt area in areaClearList)
            {
                Image.FillArea(clearPixel, area, true);
            }

            areaClearList.Clear();
        }

        private void Render()
        {
            SmartClear();

            UpdateRenderList();

            CacheGridArea();
            LoadRenderListToWorldSpace();
        }

        private void UpdateRenderList()
        {
            CacheCameraWorldAlignedAreas();

            RepopulateRenderList();

            SortRenderList();
        }

        private void RepopulateRenderList()
        {
            renderList.Clear();

            if (!HasCamera || AnyCachedCameraWorldAlignedAreaOverlapsWorldSpace())
            {
                foreach (SCEObject obj in ObjectList)
                {
                    if (obj.IsActive)
                    {
                        TryAddRenderableComponents(obj);
                    }
                }
            }
        }

        private void TryAddRenderableComponents(SCEObject obj)
        {
            foreach (IComponent component in obj.CContainer)
            {
                if (component.IsActive && component is IRenderable renderable)
                {
                    TryAddToRenderList(renderable.GetImage(), obj.GridPosition);
                }
            }
        }

        private void TryAddToRenderList(Image image, Vector2Int offset)
        {
            if (image.IsActive && ((!HasCamera && CachedGridArea.OverlapsWith(image.AlignedArea + offset)) || AnyCachedCameraWorldAlignedAreaOverlapsArea(image.AlignedArea + offset)))
            {
                renderList.Add(new ImageRenderPackage(image, offset));
            }
        }

        private void SortRenderList()
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 1; i < renderList.Count; i++)
                {
                    ImageRenderPackage current = renderList[i], last = renderList[i - 1];

                    if (last.Image.Layer > current.Image.Layer)
                    {
                        (renderList[i], renderList[i - 1]) = (last, current);

                        swapped = true;
                    }
                }
            }
            while (swapped);
        }

        private void LoadRenderListToWorldSpace()
        {
            foreach (ImageRenderPackage renderPackage in renderList)
            {
                if (HasCamera)
                {
                    foreach (Camera camera in CameraList)
                    {
                        MapImageComponentWithinArea(renderPackage, camera.WorldAlignedArea);
                    }
                }
                else
                {
                    MapImageComponentWithinArea(renderPackage, CachedGridArea);
                }
            }
        }

        private void MapImageComponentWithinArea(ImageRenderPackage renderPackage, Area2DInt area)
        {
            Vector2Int alignedImagePosition = renderPackage.OffsetAlignedPosition;

            Area2DInt alignedResizedImageArea = area.TrimArea(renderPackage.OffsetAlignedArea);

            Area2DInt resizedImageGridArea = alignedResizedImageArea - alignedImagePosition;

            Image.MapToArea(renderPackage.Image, resizedImageGridArea, alignedImagePosition, true);

            areaClearList.Add(alignedResizedImageArea);
        }
        
        // Cache functions
        private void CacheGridArea()
        {
            CachedGridArea = Image.GridArea;
        }

        // Camera functions
        private void CacheCameraWorldAlignedAreas()
        {
            cameraWorldAlignedAreaCacheList.Clear();

            foreach (Camera camera in cameraList)
            {
                cameraWorldAlignedAreaCacheList.Add(camera.WorldAlignedArea);
            }
        }

        private bool AnyCachedCameraWorldAlignedAreaOverlapsArea(Area2DInt alignedImageArea)
        {
            foreach (Area2DInt worldAlignedArea in cameraWorldAlignedAreaCacheList)
            {
                if (worldAlignedArea.OverlapsWith(alignedImageArea))
                {
                    return true;
                }
            }

            return false;
        }

        private bool AnyCachedCameraWorldAlignedAreaOverlapsWorldSpace()
        {
            return AnyCachedCameraWorldAlignedAreaOverlapsArea(CachedGridArea);
        }
    }
}
