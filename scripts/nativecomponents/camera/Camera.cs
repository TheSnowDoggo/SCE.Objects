namespace SCE
{
    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : UIBaseExt, ICContainerHolder, IComponent, IUpdate
    {
        private const string DEFAULT_NAME = "camera";

        private const SCEColor DEFAULT_BGCOLOR = SCEColor.Black;

        #region Camera
        private readonly List<SpritePackage> renderList = new();

        private readonly Queue<Rect2D> clearQueue = new();

        private SCEColor? renderedBgColor = null;
        #endregion

        private CContainer? container;

        public Camera(string name, int width, int height, CGroup? components = null)
            : base(name, width, height)
        {
            Components = new(this, components);
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

        public Vector2 WorldPosition { get; set; }

        private readonly PropertyMemoizer<Vector2, Vector2Int> rPosMemoizer =
            new((worldPos) => worldPos.Round().ToVector2Int() * new Vector2Int(2, 1));

        public Vector2Int RenderPosition() => rPosMemoizer.Get(WorldPosition);

        private readonly PropertyMemoizer<Vector2Int, Rect2D, Rect2D> rAreaMemoizer =
            new((rPos, gridArea) => gridArea + rPos);

        public Rect2D RenderArea() => rAreaMemoizer.Get(RenderPosition(), _dpMap.GridArea);

        #endregion

        #region Settings
        public SCEColor BgColor { get; set; } = DEFAULT_BGCOLOR;

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
            Rect2D trimmedGridArea = RenderArea().TrimArea(irp.OffsetArea) - irp.Offset;

            Vector2Int cameraOffsetPosition = irp.Offset - RenderPosition();

            _dpMap.MapToArea(irp.DisplayMap, trimmedGridArea, cameraOffsetPosition, true);

            clearQueue.Enqueue(trimmedGridArea + cameraOffsetPosition);
        }

        private void SmartClear()
        {
            if (renderedBgColor is not null && renderedBgColor != BgColor)
            {
                foreach (var area in clearQueue)
                    _dpMap.Data.FillArea(new Pixel(BgColor), area);
            }
            else
            {
                _dpMap.Data.Fill(new Pixel(BgColor));
                renderedBgColor = BgColor;
            }

            clearQueue.Clear();
        }
        #endregion

        public bool Overlaps(Vector2Int start, Vector2Int end)
        {
            return RenderArea().Overlaps(start, end);
        }

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
