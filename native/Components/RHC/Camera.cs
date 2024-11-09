namespace SCECorePlus.Components.RHS
{
    using SCEComponents;

    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : IRenderable, ICContainerHolder
    {
        private const bool DefaultActiveState = true;

        private const bool DefaultUpdateOnRender = false;

        private const Color DefaultBgColor = Color.Black;

        private readonly Image image;

        public readonly List<ImageRenderPackage> renderList = new();

        private readonly Queue<Area2DInt> clearQueue = new();

        private Vector2 worldPosition;

        private Color? renderedBgColor = null;

        private Color bgColor = DefaultBgColor;

        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, CList cList)
        {
            image = new(dimensions);

            WorldSpace = worldSpace;

            CContainer = new(this, cList);

            OnUpdateWorldPosition();
        }

        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions)
            : this(worldSpace, dimensions, new CList())
        {
        }

        public bool IsActive { get; set; } = DefaultActiveState;

        public CContainer CContainer { get; }

        public Vector2Int Position
        {
            get => image.Position;
            set => image.Position = value;
        }

        public Vector2Int Dimensions { get => image.Dimensions; }

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

        /// <summary>
        /// Gets or sets the WorldSpace of this instance to render from.
        /// </summary>
        public WorldSpaceRHC WorldSpace { get; set; }

        public Color BgColor
        {
            get => bgColor;
            set
            {
                if (value == Color.Transparent)
                {
                    throw new ArgumentException("Background color cannot be Transparent.");
                }

                bgColor = value;
            }
        }

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
        public Image GetImage()
        {
            return image;
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

        public void Resize(Vector2Int dimensions)
        {
            if (dimensions <= 0)
            {
                throw new ArgumentException("Dimensions cannot be less than 0.");
            }

            image.CleanResize(dimensions);

            renderList.Clear();
            clearQueue.Clear();

            renderedBgColor = null;

            OnUpdateWorldPosition();
        }

        private void SmartClear()
        {
            Pixel clearPixel = new(BgColor);
            if (renderedBgColor != null && renderedBgColor != BgColor)
            {
                foreach (Area2DInt area in clearQueue)
                {
                    image.FillArea(clearPixel, area);
                }
            }
            else
            {
                image.Fill(clearPixel);
                renderedBgColor = BgColor;
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

                image.MapToArea(irp.Image, trimmedGridArea, cameraOffsetPosition, true);

                clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
            }
        }

        private void OnUpdateWorldPosition()
        {
            WorldPositionInt = (Vector2Int)WorldPosition.Round();

            WorldPositionIntCorner = WorldPositionInt + Dimensions;

            WorldAlignedArea = image.GridArea + WorldPositionInt;
        }
    }
}
