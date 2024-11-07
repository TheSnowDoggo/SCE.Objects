namespace SCECorePlus.Components.RHS
{
    using SCEComponents;

    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class CameraV2 : Image, IRenderable, ICContainerHolder
    {
        private const bool DefaultActiveState = true;
        private const bool DefaultUpdateOnRender = false;

        public readonly List<ImageRenderPackage> renderList = new();

        private readonly Queue<Area2DInt> clearQueue = new();

        private Vector2 worldPosition;

        private Color? renderedBgColor = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="dimensions">The dimensions of the camera.</param>
        /// <param name="cList">The initial cList of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public CameraV2(WorldSpaceRHCV2 worldSpace, Vector2Int dimensions, CList cList, bool isActive = DefaultActiveState)
            : base(dimensions, isActive)
        {
            WorldSpace = worldSpace;

            CContainer = new(this, cList);

            OnUpdateWorldPosition();

            OnResize += Camera_OnImageResize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="dimensions">The dimensions of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public CameraV2(WorldSpaceRHCV2 worldSpace, Vector2Int dimensions, bool isActive = DefaultActiveState)
            : this(worldSpace, dimensions, new CList(), isActive)
        {
        }

        /// <summary>
        /// Gets or sets the position of this instance in the WorldSpace.
        /// </summary>
        public Vector2 WorldPosition
        {
            get => worldPosition;
            set
            {
                worldPosition = value;
                OnUpdateWorldPosition();
            }
        }

        public CContainer CContainer { get; }

        /// <summary>
        /// Gets or sets the WorldSpace of this instance to render from.
        /// </summary>
        public WorldSpaceRHCV2 WorldSpace { get; set; }

        /// <summary>
        /// Gets the world position rounded and converted to the nearest <see cref="Vector2Int"/>.
        /// </summary>
        public Vector2Int WorldPositionInt { get; private set; }

        /// <summary>
        /// Gets the world position rounded and converted to the nearest <see cref="Vector2Int"/>.
        /// </summary>
        public Vector2Int WorldPositionIntCorner { get; private set; }

        /// <summary>
        /// Gets the GridArea of this instance aligned to its WorldPosition.
        /// </summary>
        public Area2DInt WorldAlignedArea { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera should update every component on render.
        /// </summary>
        public bool UpdateOnRender { get; set; } = DefaultUpdateOnRender;

        /// <inheritdoc/>
        public override Image GetImage()
        {
            return this;
        }

        public void LoadIRP(ImageRenderPackage irp)
        {
            renderList.Add(irp);
        }

        public void Render()
        {
            SmartClear();
            SortRenderList();
            LoadRenderList();
            renderList.Clear();
        }

        private void SmartClear()
        {
            Color bgColor = WorldSpace.BgColor;

            if (bgColor == Color.Transparent)
            {
                throw new Exception("Transparent is not a valid background color.");
            }

            Pixel clearPixel = new(bgColor);
            if (renderedBgColor != null && renderedBgColor != bgColor)
            {
                foreach (Area2DInt area in clearQueue)
                {
                    FillArea(clearPixel, area);
                }
            }
            else
            {
                Fill(clearPixel);
                renderedBgColor = bgColor;
            }

            clearQueue.Clear();
        }

        private void SortRenderList()
        {
            renderList.Sort((a, b) => a.Image.Layer < b.Image.Layer ? -1 : 1);
        }

        private void LoadRenderList()
        {
            foreach (ImageRenderPackage irp in renderList)
            {
                RenderIRP(irp);
            }
        }

        /// <summary>
        /// Renders the specified irp to the camera.
        /// </summary>
        private void RenderIRP(ImageRenderPackage irp)
        {
            if (Area2DInt.Overlaps(WorldAlignedArea, irp.AlignedArea))
            {
                Area2DInt trimmedGridArea = WorldAlignedArea.TrimArea(irp.AlignedArea) - irp.AlignedPosition;

                Vector2Int cameraOffsetPosition = irp.AlignedPosition - WorldPositionInt;

                MapToArea(irp.Image, trimmedGridArea, cameraOffsetPosition, true);

                clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
            }
        }

        private void OnUpdateWorldPosition()
        {
            WorldPositionInt = (Vector2Int)WorldPosition.Round();

            WorldPositionIntCorner = WorldPositionInt + Dimensions;

            WorldAlignedArea = GridArea + WorldPositionInt;
        }

        private void Camera_OnImageResize()
        {
            OnUpdateWorldPosition();

            renderList.Clear();
            clearQueue.Clear();
            renderedBgColor = null;
        }
    }
}
