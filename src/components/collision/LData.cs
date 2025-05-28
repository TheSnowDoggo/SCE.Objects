using CSUtils;
namespace SCE
{
    public static class LData
    {
        #region Layers

        public const uint LAYER_1 = 0x0000_0001;
        public const uint LAYER_2 = 0x0000_0002;
        public const uint LAYER_3 = 0x0000_0004;
        public const uint LAYER_4 = 0x0000_0008;
        public const uint LAYER_5 = 0x0000_0010;
        public const uint LAYER_6 = 0x0000_0020;
        public const uint LAYER_7 = 0x0000_0040;
        public const uint LAYER_8 = 0x0000_0080;
        public const uint LAYER_9 = 0x0000_0100;
        public const uint LAYER_10 = 0x0000_0200;
        public const uint LAYER_11 = 0x0000_0400;
        public const uint LAYER_12 = 0x0000_0800;
        public const uint LAYER_13 = 0x0000_1000;
        public const uint LAYER_14 = 0x0000_2000;
        public const uint LAYER_15 = 0x0000_4000;
        public const uint LAYER_16 = 0x0000_8000;
        public const uint LAYER_ALL = 0x0000_FFFF;

        #endregion

        #region Masks

        public const uint MASK_1 = 0x0001_0000;
        public const uint MASK_2 = 0x0002_0000;
        public const uint MASK_3 = 0x0004_0000;
        public const uint MASK_4 = 0x0008_0000;
        public const uint MASK_5 = 0x0010_0000;
        public const uint MASK_6 = 0x0020_0000;
        public const uint MASK_7 = 0x0040_0000;
        public const uint MASK_8 = 0x0080_0000;
        public const uint MASK_9 = 0x0100_0000;
        public const uint MASK_10 = 0x0200_0000;
        public const uint MASK_11 = 0x0400_0000;
        public const uint MASK_12 = 0x0800_0000;
        public const uint MASK_13 = 0x1000_0000;
        public const uint MASK_14 = 0x2000_0000;
        public const uint MASK_15 = 0x4000_0000;
        public const uint MASK_16 = 0x8000_0000;
        public const uint MASK_ALL = 0xFFFF_0000;

        #endregion

        public static bool ContainsBit(uint data, int bit)
        {
            if (bit < 0 || bit >= 32)
            {
                return false;
            }
            var p = (uint)1 << bit;
            return (data & p) == p;
        }

        public static IEnumerable<int> EnumerateLayers(uint data)
        {
            for (int i = 0; i < 16; ++i)
            {
                if (ContainsBit(data ,i))
                {
                    yield return i + 1;
                }
            }
        }

        public static IEnumerable<int> EnumerateMasks(uint data)
        {
            for (int i = 16; i < 32; ++i)
            {
                if (ContainsBit(data, i))
                {
                    yield return i - 15;
                }
            }
        }

        public static string Build(uint data)
        {
            return Utils.FTL(Convert.ToString(data, 2), 32, '0', Utils.FMode.Left).Insert(16, "_");
        }
    }
}
