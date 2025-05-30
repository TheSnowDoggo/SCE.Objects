﻿using CSUtils;
using System.Collections;

namespace SCE
{
    public class AnimationSprite2D : ComponentBase<SCEObject>, IEnumerable<DisplayMap>, IRenderable, ISmartLayerable
    {
        private readonly List<DisplayMap> _dpMapList;

        private int current = 0;

        public AnimationSprite2D(IEnumerable<DisplayMap> dpMapList)
            : base()
        {
            _dpMapList = new(dpMapList);
        }

        public AnimationSprite2D()
            : this(new List<DisplayMap>())
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

        #region IEnumerable

        public IEnumerator<DisplayMap> GetEnumerator()
        {
            return _dpMapList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public DisplayMapView GetMapView()
        {
            return (DisplayMapView)Selected;
        }

        #region Modify

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
            Current = Utils.Mod(Current + 1, Count);
        }

        public void Previous()
        {
            Current = Utils.Mod(Current- 1, Count);
        }
    }
}
