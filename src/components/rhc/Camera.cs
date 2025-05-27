namespace SCE
{
    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : SCEObject, IRenderable, IComponent
    {
        private readonly DisplayMap _dpMap;

        private readonly Queue<Rect2DInt> clearQueue = new();

        private CContainer? container;

        private Pixel? renderedPixel = null;

        public Camera(int width, int height, params IComponent[] arr)
            : base(arr)
        {
            _dpMap = new(width, height);
        }

        public Camera(Vector2Int dimensions, params IComponent[] arr)
            : this(dimensions.X, dimensions.Y, arr)
        {
        }

        public IUpdateLimit? UpdateLimiter { get; set; }

        public CContainer? Container { get => container; }

        public WorldSpaceRHC? WorldSpace { get => (WorldSpaceRHC?)Container?.Holder; }

        public Pixel BasePixel { get; set; }

        public bool ConsistantSorting { get; set; } = true;

        public int Width { get => _dpMap.Width; }

        public int Height { get => _dpMap.Height; }

        public Vector2Int Dimensions { get => _dpMap.Dimensions; }

        public Vector2Int Offset { get; set; }

        public int Layer { get; set; }

        public Anchor Anchor { get; set; }

        public Vector2Int RenderPosition() { return GlobalGridPosition() * new Vector2Int(2, 1); }

        public Rect2DInt RenderArea() { return _dpMap.GridArea() + RenderPosition(); }

        #region Resize

        public void Resize(int width, int height)
        {
            _dpMap.CleanResize(width, height);

            clearQueue.Clear();

            renderedPixel = null;
        }

        public void Resize(Vector2Int dimensions)
        {
            Resize(dimensions.X, dimensions.Y);
        }

        #endregion

        #region Render

        public void Render(List<SpritePackage> renderList)
        {
            SmartClear();

            if (ConsistantSorting)
            {
                bool swapped;
                int top = renderList.Count;
                do
                {
                    swapped = false;
                    for (int i = 1; i < top; ++i)
                    {
                        if (renderList[i - 1].CompareTo(renderList[i]) > 0)
                        {
                            (renderList[i - 1], renderList[i]) = (renderList[i], renderList[i - 1]);
                            swapped = true;
                        }
                    }
                    --top;
                }
                while (swapped);
            }
            else
            {
                renderList.Sort();
            }

            var renderPos = RenderPosition();

            var renderArea = RenderArea();

            foreach (var irp in renderList)
            {
                Rect2DInt trimmedGridArea = renderArea.TrimArea(irp.OffsetArea) - irp.Offset;

                Vector2Int cameraOffsetPosition = irp.Offset - renderPos;

                _dpMap.PMapTo(irp.DisplayMapView, trimmedGridArea, cameraOffsetPosition);

                clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
            }

            renderList.Clear();
        }

        #endregion

        #region Clear

        public void Clear()
        {
            _dpMap.Fill(BasePixel);
        }

        private void SmartClear()
        {
            if (renderedPixel != null && renderedPixel == BasePixel)
            {
                foreach (var area in clearQueue)
                {
                    _dpMap.Fill(BasePixel, area);
                }
            }
            else
            {
                Clear();
                renderedPixel = BasePixel;
            }

            clearQueue.Clear();
        }

        #endregion

        /// <inheritdoc/>
        public void SetCContainer(CContainer? container, ICContainerHolder holder)
        {
            if (holder is not WorldSpaceRHC)
            {
                throw new InvalidCContainerHolderException("Holder must be WorldSpaceRHC.");
            }
            this.container = container;
        }

        /// <inheritdoc/>
        public DisplayMapView GetMapView()
        {
            return (DisplayMapView)_dpMap;
        }
    }
}
