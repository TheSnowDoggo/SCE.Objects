namespace SCE
{
    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : UIBaseExt, ICContainerHolder, IComponent, IUpdate
    {
        private const string DEFAULT_NAME = "camera";

        private const Color DEFAULT_BGCOLOR = Color.Black;

        #region Camera
        private readonly List<SpritePackage> renderList = new();

        private readonly Queue<Area2DInt> clearQueue = new();

        private Vector2 worldPosition;

        private Color? renderedBgColor = null;
        #endregion

        private CContainer? container;

        public Camera(string name, int width, int height, CGroup? components = null)
            : base(name, width, height)
        {
            Components = new(this, components);

            UpdateWorldProperties();
        }

        public Camera(string name, Vector2Int dimensions, CGroup? components = null)
            : this(name, dimensions.X, dimensions.Y, components)
        {
        }

        public Camera(int width, int height, CGroup? components = null)
            : this(DEFAULT_NAME, width, height, components)
        {
        }

        public Camera(Vector2Int dimensions, CGroup? components = null)
            : this(DEFAULT_NAME, dimensions, components)
        {
        }

        public CContainer Components { get; }

        public CContainer Container { get => container ?? throw new NullReferenceException("Container is null."); }

        public WorldSpaceRHC WorldSpace { get => (WorldSpaceRHC)Container.Holder; }

        public IUpdateLimit? UpdateLimiter { get; set; }

        #region WorldProperties
        public Vector2 WorldPosition
        {
            get => worldPosition;
            set => UpdateWorldProperties(value);
        }

        public Vector2Int WorldPositionInt { get; private set; }

        public Vector2Int WorldPositionIntCorner { get; private set; }

        public Area2DInt WorldAlignedArea { get; private set; }
        #endregion

        #region Settings
        public Color BgColor { get; set; } = DEFAULT_BGCOLOR;

        public bool ConsistantSorting { get; set; } = true;
        #endregion

        #region Load
        internal void Load(SpritePackage irp)
        {
            renderList.Add(irp);
        }
        #endregion

        #region Resize
        public void Resize(int width, int height)
        {
            _dpMap.CleanResize(width, height);

            renderList.Clear();
            clearQueue.Clear();

            renderedBgColor = null;

            UpdateWorldProperties();
        }

        public void Resize(Vector2Int dimensions)
        {
            Resize(dimensions.X, dimensions.Y);
        }
        #endregion

        #region Sorting
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
        #endregion

        #region Render
        internal void RenderNow()
        {
            SmartClear();
            SortRenderList();
            RenderAll();
            renderList.Clear();
        }

        private void RenderAll()
        {
            foreach (var irp in renderList)
                RenderIRP(irp);
        }

        private void RenderIRP(SpritePackage irp)
        {
            Area2DInt trimmedGridArea = WorldAlignedArea.TrimArea(irp.OffsetArea) - irp.Offset;

            Vector2Int cameraOffsetPosition = irp.Offset - WorldPositionInt;

            _dpMap.MapToArea(irp.DisplayMap, trimmedGridArea, cameraOffsetPosition, true);

            clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
        }

        private void SmartClear()
        {
            if (renderedBgColor is not null && renderedBgColor != BgColor)
            {
                foreach (Area2DInt area in clearQueue)
                    _dpMap.FillArea(new Pixel(BgColor), area);
            }
            else
            {
                _dpMap.Fill(new Pixel(BgColor));
                renderedBgColor = BgColor;
            }

            clearQueue.Clear();
        }
        #endregion

        #region WorldPropertyFunc
        private void UpdateWorldProperties(Vector2? newWorldPos = null)
        {
            if (newWorldPos is Vector2 newPosition)
                worldPosition = newPosition;

            WorldPositionInt = (Vector2Int)WorldPosition.Round();

            WorldPositionIntCorner = WorldPositionInt + Dimensions;

            WorldAlignedArea = _dpMap.GridArea + WorldPositionInt;
        }
        #endregion

        #region Update
        public void Update()
        {
            if (!UpdateLimiter?.OnUpdate() ?? false)
                return;
            Components.Update();
        }
        #endregion

        #region ComponentFuncs
        public void SetCContainer(CContainer? container, ICContainerHolder holder)
        {
            if (holder is not WorldSpaceRHC)
                throw new InvalidCContainerHolderException();
            this.container = container;
        }
        #endregion
    }
}
