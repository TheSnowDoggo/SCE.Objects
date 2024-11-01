namespace SCECorePlus.Components.RHS
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
        public Vector2Int OffsetAlignedPosition { get => Image.Position + Offset; }

        /// <summary>
        /// Gets the image aligned position corner offset by the offset.
        /// </summary>
        public Vector2Int OffsetAlignedPositionCorner { get => Image.Position + Image.Dimensions + Offset; }

        /// <summary>
        /// Gets the image aligned area offset by the offset.
        /// </summary>
        public Area2DInt OffsetAlignedArea { get => new(OffsetAlignedPosition, OffsetAlignedPositionCorner); }
    }
}
