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

        private Vector2 worldPosition;

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

        public void FillBackground(Color bgColor)
        {
            BgColorFill(bgColor);
        }

        /// <summary>
        /// Renders the specified irp to the camera.
        /// </summary>
        public void RenderIRP(ImageRenderPackage irp)
        {
            Area2DInt area = irp.AlignedArea;

            Area2DInt trimmedGridArea = WorldAlignedArea.TrimArea(area) - irp.Offset;

            MapAreaFrom(irp.Image, trimmedGridArea, irp.Offset, true);
        }

        private void OnUpdateWorldPosition()
        {
            WorldPositionInt = (Vector2Int)WorldPosition.Round();

            WorldAlignedArea = GridArea + WorldPositionInt;
        }
    }
}
