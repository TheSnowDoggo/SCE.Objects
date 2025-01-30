namespace SCE
{
    /// <summary>
    /// A struct used for containing an image and its offset for rendering.
    /// </summary>
    internal class SpritePackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpritePackage"/> struct.
        /// </summary>
        /// <param name="dpMap">The displaymap to render.</param>
        /// <param name="offset">The offset of the image to render.</param>
        public SpritePackage(DisplayMap dpMap, int layer, Vector2Int offset)
        {
            DisplayMap = dpMap;
            Layer = layer;
            Offset = offset;          

            OffsetCorner = offset + DisplayMap.Dimensions;
            OffsetArea = new(offset, OffsetCorner); 
        }

        /// <summary>
        /// Gets the displayMap to render.
        /// </summary>
        public DisplayMap DisplayMap { get; }

        public int Layer { get; }

        /// <summary>
        /// Gets the offset of the image to render.
        /// </summary>
        public Vector2Int Offset { get; }

        /// <summary>
        /// Gets the image aligned position corner offset by the offset.
        /// </summary>
        public Vector2Int OffsetCorner { get; }

        /// <summary>
        /// Gets the image aligned area offset by the offset.
        /// </summary>
        public Rect2D OffsetArea { get; }
    }
}
