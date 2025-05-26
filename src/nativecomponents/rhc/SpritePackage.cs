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

            OffsetArea = new(offset, offset + DisplayMap.Dimensions); 
        }

        public DisplayMap DisplayMap { get; }

        public int Layer { get; }

        public Vector2Int Offset { get; }

        public Rect2D OffsetArea { get; }
    }
}
