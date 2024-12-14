namespace SCE
{
    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : IRenderable, ICContainerHolder
    {
        private const Color DefaultBgColor = Color.Black;

        private readonly DisplayMap dpMap;

        private readonly List<RenderPackage> renderList = new();

        private readonly Queue<Area2DInt> clearQueue = new();

        private Vector2 worldPosition;

        private Color? renderedBgColor = null;

        private Color bgColor = DefaultBgColor;

        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, CList cList)
        {
            dpMap = new(dimensions);

            WorldSpace = worldSpace;

            CContainer = new(this, cList);

            OnUpdateWorldPosition();
        }

        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions)
            : this(worldSpace, dimensions, new CList())
        {
        }

        public bool IsActive { get; set; } = true;

        public CContainer CContainer { get; }

        public Vector2Int Position { get; set; }

        public int Layer { get; set; }

        public Vector2Int Dimensions { get => dpMap.Dimensions; }

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
        public bool UpdateOnRender { get; set; } = false;

        /// <inheritdoc/>
        public DisplayMap GetMap()
        {
            return dpMap;
        }

        internal void LoadIRP(RenderPackage irp)
        {
            renderList.Add(irp);
        }

        internal void Render()
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

            dpMap.CleanResize(dimensions);

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
                    dpMap.FillArea(clearPixel, area);
                }
            }
            else
            {
                dpMap.Fill(clearPixel);
                renderedBgColor = BgColor;
            }

            clearQueue.Clear();
        }

        private void SortRenderList()
        {
            renderList.Sort((a, b) => a.Layer < b.Layer ? -1 : 1);
        }

        private void LoadRenderList()
        {
            foreach (RenderPackage irp in renderList)
            {
                RenderIRP(irp);
            }
        }

        /// <summary>
        /// Renders the specified irp to the camera.
        /// </summary>
        private void RenderIRP(RenderPackage irp)
        {
            if (Area2DInt.Overlaps(WorldAlignedArea, irp.OffsetArea))
            {
                Area2DInt trimmedGridArea = WorldAlignedArea.TrimArea(irp.OffsetArea) - irp.Offset;

                Vector2Int cameraOffsetPosition = irp.Offset - WorldPositionInt;

                dpMap.MapToArea(irp.DisplayMap, trimmedGridArea, cameraOffsetPosition, true);

                clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
            }
        }

        private void OnUpdateWorldPosition()
        {
            WorldPositionInt = (Vector2Int)WorldPosition.Round();

            WorldPositionIntCorner = WorldPositionInt + Dimensions;

            WorldAlignedArea = dpMap.GridArea + WorldPositionInt;
        }
    }
}
