using CSUtils;
namespace SCE
{
    public struct LInfo : IEquatable<LInfo>
    {
        #region Layers

        public const uint LAYER_1  = 0x0000_0001;
        public const uint LAYER_2  = 0x0000_0002;
        public const uint LAYER_3  = 0x0000_0004;
        public const uint LAYER_4  = 0x0000_0008;
        public const uint LAYER_5  = 0x0000_0010;
        public const uint LAYER_6  = 0x0000_0020;
        public const uint LAYER_7  = 0x0000_0040;
        public const uint LAYER_8  = 0x0000_0080;
        public const uint LAYER_9  = 0x0000_0100;
        public const uint LAYER_10 = 0x0000_0200;
        public const uint LAYER_11 = 0x0000_0400;
        public const uint LAYER_12 = 0x0000_0800;
        public const uint LAYER_13 = 0x0000_1000;
        public const uint LAYER_14 = 0x0000_2000;
        public const uint LAYER_15 = 0x0000_4000;
        public const uint LAYER_16 = 0x0000_8000;
        public const uint LAYER_ALL= 0x0000_FFFF;

        #endregion

        #region Masks

        public const uint MASK_1  = 0x0001_0000;
        public const uint MASK_2  = 0x0002_0000;
        public const uint MASK_3  = 0x0004_0000;
        public const uint MASK_4  = 0x0008_0000;
        public const uint MASK_5  = 0x0010_0000;
        public const uint MASK_6  = 0x0020_0000;
        public const uint MASK_7  = 0x0040_0000;
        public const uint MASK_8  = 0x0080_0000;
        public const uint MASK_9  = 0x0100_0000;
        public const uint MASK_10 = 0x0200_0000;
        public const uint MASK_11 = 0x0400_0000;
        public const uint MASK_12 = 0x0800_0000;
        public const uint MASK_13 = 0x1000_0000;
        public const uint MASK_14 = 0x2000_0000;
        public const uint MASK_15 = 0x4000_0000;
        public const uint MASK_16 = 0x8000_0000;
        public const uint MASK_ALL= 0xFFFF_0000;

        #endregion

        private uint data;

        public LInfo(uint values = 0)
        {
            data = values;
        }

        public static explicit operator uint(LInfo lInfo) => lInfo.data;

        public static bool operator ==(LInfo left, LInfo right) => left.Equals(right);

        public static bool operator !=(LInfo left, LInfo right) => !left.Equals(right);

        #region Equality

        public bool Equals(LInfo other)
        {
            return data == other.data;
        }

        public override bool Equals(object? obj)
        {
            return obj is LInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(data);
        }

        #endregion

        public static uint ToValue(int id)
        {
            if (id < 0 || id > 31)
                throw new ArgumentException("Id must be between 1-31");
            return (uint)1 << id;
        }

        #region Add

        public void Add(uint values)
        {
            data += values;
        }

        public void AddID(int id)
        {
            Add(ToValue(id));
        }

        public void AddID(LID id)
        {
            AddID((int)id);
        }

        public void AddSelectionID(int startId, int count)
        {
            AddRangeID(Enumerable.Range(startId, count));
        }

        public void AddSelectionID(LID startId, int count)
        {
            AddSelectionID((int)startId, count);
        }

        public void AddRangeID(IEnumerable<int> collection)
        {
            foreach (var id in collection)
                AddID(id);
        }

        public void AddRangeID(IEnumerable<LID> collection)
        {
            foreach (var id in collection)
                AddID(id);
        }

        public void AddEveryID(params int[] idArr)
        {
            AddRangeID(idArr);
        }

        public void AddEveryID(params LID[] idArr)
        {
            AddRangeID(idArr);
        }

        #endregion

        #region Remove

        public bool Remove(uint values)
        {
            if (values > data)
                return false;
            data -= values;
            return true;
        }

        public bool RemoveID(int id)
        {
            var value = ToValue(id);
            if (ContainsAll(value))
                return false;
            data -= value;
            return true;
        }

        public bool RemoveID(LID id)
        {
            return RemoveID((int)id);
        }

        public void RemoveSelectionID(int startId, int count)
        {
            RemoveRangeID(Enumerable.Range(startId, count));
        }

        public void RemoveSelectionID(LID startId, int count)
        {
            RemoveSelectionID((int)startId, count);
        }

        public void RemoveRangeID(IEnumerable<int> collection)
        {
            foreach (var id in collection)
                RemoveID(id);
        }

        public void RemoveRangeID(IEnumerable<LID> collection)
        {
            foreach (var id in collection)
                RemoveID(id);
        }

        public void RemoveEveryID(params int[] idArr)
        {
            RemoveRangeID(idArr);
        }

        public void RemoveEveryID(params LID[] idArr)
        {
            RemoveRangeID(idArr);
        }

        #endregion

        #region Validation

        public bool ContainsAll(uint values)
        {
            return (data & values) == values;
        }

        public bool ContainsAny(uint values)
        {
            return (data & values) > 0;
        }

        public bool ContainsID(int id)
        {
            return ContainsAll(ToValue(id));
        }

        public bool ContainsID(LID id)
        {
            return ContainsID((int)id);
        }

        public bool CollidesWith(LInfo other)
        {
            return ((data >> 16) & other.data & LAYER_ALL) > 0;
        }

        #endregion

        public override string ToString()
        {
            return Utils.FTL(Convert.ToString(data, 2), 32, '0', Utils.FMode.Left).Insert(16, "_");
        }
    }
}