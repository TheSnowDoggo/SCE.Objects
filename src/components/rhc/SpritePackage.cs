namespace SCE
{
    /// <summary>
    /// A struct used for containing an image and its offset for rendering.
    /// </summary>
    public class SpritePackage : IComparable<SpritePackage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpritePackage"/> struct.
        /// </summary>
        public SpritePackage(DisplayMapView dpMap, int layer, Vector2Int offset)
        {
            DisplayMapView = dpMap;
            Layer = layer;
            Offset = offset;
            OffsetArea = new(offset, offset + DisplayMapView.Dimensions);
        }

        public DisplayMapView DisplayMapView { get; }

        public int Layer { get; }

        public Vector2Int Offset { get; }

        public Rect2DInt OffsetArea { get; }

        public int CompareTo(SpritePackage? other)
        {
            if (other is null)
            {
                throw new NotImplementedException();
            }
            return Layer - other.Layer; // Higher = More visible
        }
    }
}
