namespace SCE
{
    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : UIBaseExt, ICContainerHolder
    {
        private const Color DefaultBgColor = Color.Black;

        private readonly List<SpritePackage> renderList = new();

        private readonly Queue<Area2DInt> clearQueue = new();

        private Vector2 worldPosition;

        private Color? renderedBgColor = null;

        private Color bgColor = DefaultBgColor;

        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, CGroup cList)
            : base(dimensions)
        {
            WorldSpace = worldSpace;

            Components = new(this, cList);

            OnUpdateWorldPosition();
        }

        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions)
            : this(worldSpace, dimensions, new CGroup())
        {
        }

        public CContainer Components { get; }

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
                    throw new ArgumentException("Background color cannot be Transparent.");
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

        public bool ConsistantSorting { get; set; } = true;

        internal void LoadIRP(SpritePackage irp)
        {
            renderList.Add(irp);
        }

        internal void RenderNow()
        {
            SmartClear();
            SortRenderList();
            LoadRenderList();
            renderList.Clear();
        }

        public void Resize(Vector2Int dimensions)
        {
            if (dimensions <= 0)
                throw new ArgumentException("Dimensions cannot be less than 0.");

            _dpMap.CleanResize(dimensions);

            renderList.Clear();
            clearQueue.Clear();

            renderedBgColor = null;

            OnUpdateWorldPosition();
        }

        private void SmartClear()
        {
            Pixel clearPixel = new(BgColor);
            if (renderedBgColor is not null && renderedBgColor != BgColor)
            {
                foreach (Area2DInt area in clearQueue)
                    _dpMap.FillArea(clearPixel, area);
            }
            else
            {
                _dpMap.Fill(clearPixel);
                renderedBgColor = BgColor;
            }

            clearQueue.Clear();
        }
        private void QuickSortRenderList()
        {
            renderList.Sort((a, b) => a.Layer < b.Layer ? -1 : 1);
        }

        private void BubbleSortRenderList()
        { 
            bool swapped;
            int top = renderList.Count;
            do
            {
                swapped = false;
                for (int i = 1; i < top; ++i)
                {
                    if (renderList[i - 1].Layer > renderList[i].Layer)
                    {
                        (renderList[i - 1], renderList[i]) = (renderList[i], renderList[i - 1]);
                        swapped = true;
                    }
                }
                --top;
            }
            while (swapped);
        }

        private void SortRenderList()
        {
            if (ConsistantSorting)
                BubbleSortRenderList();
            else
                QuickSortRenderList();
        }

        private void LoadRenderList()
        {
            foreach (SpritePackage irp in renderList)
                RenderIRP(irp);
        }

        private void RenderIRP(SpritePackage irp)
        {
            Area2DInt trimmedGridArea = WorldAlignedArea.TrimArea(irp.OffsetArea) - irp.Offset;

            Vector2Int cameraOffsetPosition = irp.Offset - WorldPositionInt;

            _dpMap.MapToArea(irp.DisplayMap, trimmedGridArea, cameraOffsetPosition, true);

            clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
        }

        private void OnUpdateWorldPosition()
        {
            WorldPositionInt = (Vector2Int)WorldPosition.Round();

            WorldPositionIntCorner = WorldPositionInt + Dimensions;

            WorldAlignedArea = _dpMap.GridArea + WorldPositionInt;
        }
    }
}
