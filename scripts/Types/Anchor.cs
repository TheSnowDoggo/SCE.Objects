namespace SCECorePlus.Types
{
    public class Anchor
    {
        private const AnchorMode DefaultAnchorMode = AnchorMode.BotttomLeft;

        public Anchor(AnchorMode mode, Vector2Int offset)
        {
            Mode = mode;
            Offset = offset;
        }

        public Anchor(AnchorMode mode)
            : this(mode, Vector2Int.Zero)
        {
        }

        public Anchor(Vector2Int offset)
            : this(DefaultAnchorMode, offset)
        {
        }

        public Anchor()
            : this(DefaultAnchorMode)
        {
        }

        /// <summary>
        /// Represents the method used to anchor the offset.
        /// </summary>
        public enum AnchorMode : byte
        {
            /// <summary>
            /// Bottom left anchoring.
            /// </summary>
            BotttomLeft,

            /// <summary>
            /// Bottom right anchoring.
            /// </summary>
            BottomRight,

            /// <summary>
            /// Top left anchoring.
            /// </summary>
            TopLeft,

            /// <summary>
            /// Top right anchoring.
            /// </summary>
            TopRight,

            /// <summary>
            /// Center anchoring.
            /// </summary>
            Center,
        }

        public AnchorMode Mode { get; set; }

        public Vector2Int Offset { get; set; }

        /// <summary>
        /// Returns the aligned offset based on the specified dimensions and the current anchor mode and offset.
        /// </summary>
        /// <param name="dimensions">The dimensions to anchor from.</param>
        /// <returns>The aligned offset based on the specified dimensions and the current anchor mode and offset.</returns>
        public Vector2Int GetAlignedOffset(Vector2Int dimensions)
        {
            Vector2Int offset = Mode switch
            {
                AnchorMode.BotttomLeft => Vector2Int.Zero,
                AnchorMode.BottomRight => new(dimensions.X - 1, 0),
                AnchorMode.TopLeft => new(0, dimensions.Y - 1),
                AnchorMode.TopRight => dimensions - 1,
                AnchorMode.Center => dimensions.Midpoint,
                _ => throw new NotImplementedException("Unknown anchor type.")
            };

            return -offset + Offset;
        }
    }
}
