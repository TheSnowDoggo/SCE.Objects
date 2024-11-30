namespace SCECorePlus
{
    /// <summary>
    /// A struct used for containing an image and its offset for rendering.
    /// </summary>
    public readonly struct ImageRenderPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderPackage"/> struct.
        /// </summary>
        /// <param name="image">The image to render.</param>
        /// <param name="offset">The offset of the image to render.</param>
        public ImageRenderPackage(Image image, Vector2Int offset)
        {
            Image = image;
            Offset = offset;

            AlignedPosition = Image.Position + Offset;
            AlignedPositionCorner = AlignedPosition + Image.Dimensions;
            AlignedArea = new(AlignedPosition, AlignedPositionCorner); 
        }

        /// <summary>
        /// Gets the image to render.
        /// </summary>
        public Image Image { get; }

        /// <summary>
        /// Gets the offset of the image to render.
        /// </summary>
        public Vector2Int Offset { get; }

        /// <summary>
        /// Gets the image aligned position offset by the offset.
        /// </summary>
        public Vector2Int AlignedPosition { get; }

        /// <summary>
        /// Gets the image aligned position corner offset by the offset.
        /// </summary>
        public Vector2Int AlignedPositionCorner { get; }

        /// <summary>
        /// Gets the image aligned area offset by the offset.
        /// </summary>
        public Area2DInt AlignedArea { get; }
    }
}
