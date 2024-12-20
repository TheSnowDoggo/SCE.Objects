namespace SCE
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An <see cref="IComponent"/> used for storing an image in an object.
    /// </summary>
    public class ImageComponent : ComponentBase<SCEObject>, IRenderable
    {
        private Image? image;


        public ImageComponent()
            : base()
        {
        }

        public ImageComponent(Image image)
            : base()
        {
            Image = image;
        }

        public Vector2Int Position { get => Image.Position; }

        public int Layer { get => Image.Layer; }

        public Anchor Anchor { get => Image.Anchor; }

        /// <summary>
        /// Gets or sets the stored Image of this instance.
        /// </summary>
        /// <remarks>
        /// <i>Note: The image may be null resulting in an exception being thrown.</i>
        /// </remarks>
        [AllowNull]
        public Image Image
        {
            get => image ?? throw new NullReferenceException("Image is null.");
            set => image = value;
        }

        /// <summary>
        /// Gets a value indicating whether this image component has an image.
        /// </summary>
        public bool HasImage { get => image != null; }

        public DisplayMap GetMap()
        {
            return Image;
        }
    }
}
