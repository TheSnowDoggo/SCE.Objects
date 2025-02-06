namespace SCE
{
    using System.Collections;

    public class AnimationSprite2D : ComponentBase<SCEObject>, IRenderable, ISmartLayerable, IEnumerable<DisplayMap>
    {
        private const string DEFAULT_NAME = "animation_sprite";

        private readonly List<DisplayMap> _dpMapList;

        private int current = 0;

        public AnimationSprite2D(string name, IEnumerable<DisplayMap> dpMapList)
            : base(name)
        {
            _dpMapList = new(dpMapList);
        }

        public AnimationSprite2D(string name)
            : this(name, new List<DisplayMap>())
        {
        }

        public AnimationSprite2D(IEnumerable<DisplayMap> dpMapList)
            : this(DEFAULT_NAME, dpMapList)
        {
        }

        public AnimationSprite2D()
            : this(DEFAULT_NAME)
        {
        }

        public DisplayMap this[int index]
        {
            get => _dpMapList[index];
            set => _dpMapList[index] = value;
        }

        public int Count { get => _dpMapList.Count; }

        public int Current
        {
            get
            {
                if (current >= Count)
                    current = Count - 1;
                return current;
            }
            set
            {
                if (value != -1 && (value < 0 || value >= Count))
                    throw new ArgumentException("Invalid value.", "Current");
                current = value;
            }
        }

        public bool IsSelected { get => Current != -1; }

        public DisplayMap? NullabeSelected
        {
            get
            {
                if (Current == -1)
                    return null;
                return this[current];
            }
        }

        public DisplayMap Selected { get => NullabeSelected ?? throw new NullReferenceException("No sprite selected."); }

        public Vector2Int Offset { get; set; }

        public Anchor Anchor { get; set; }

        public int Layer { get; set; }

        private Vector2Int CycleRange { get => new(0, Count); }

        public IEnumerator<DisplayMap> GetEnumerator()
        {
            return _dpMapList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DisplayMap GetMap()
        {
            return Selected;
        }

        #region List
        public void Add(DisplayMap dpMap)
        {
            _dpMapList.Add(dpMap);
        }

        public void Add(DisplayMap[] dpMapArr)
        {
            foreach (DisplayMap dpMap in dpMapArr)
                Add(dpMap);
        }

        public void Add(List<DisplayMap> dpMapList)
        {
            foreach (DisplayMap dpMap in dpMapList)
                Add(dpMap);
        }

        public bool Remove(DisplayMap dpMap)
        {
            return _dpMapList.Remove(dpMap);
        }

        public void RemoveAt(int index)
        {
            _dpMapList.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            _dpMapList.RemoveRange(index, count);
        }

        public int IndexOf(DisplayMap dpMap)
        {
            return _dpMapList.IndexOf(dpMap);
        }

        public bool Contains(DisplayMap dpMap)
        {
            return _dpMapList.Contains(dpMap);
        }
        #endregion

        public void Next()
        {
            Current = MathUtils.Cycle(CycleRange, Current + 1);
        }

        public void Previous()
        {
            Current = MathUtils.Cycle(CycleRange, Current - 1);
        }
    }
}
