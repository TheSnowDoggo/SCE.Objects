namespace SCECorePlus
{
    using System.Diagnostics.CodeAnalysis;

    using SCEComponents;

    /// <summary>
    /// An <see cref="IComponent"/> used for storing an image in an object.
    /// </summary>
    public class ImageComponent : IComponent, IRenderable
    {
        private const bool DefaultActiveState = true;

        private Image? image;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageComponent"/> class.
        /// </summary>
        public ImageComponent(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageComponent"/> class.
        /// </summary>
        /// <param name="image">The initial image.</param>
        public ImageComponent(string name, Image image)
            : this(name)
        {
            Image = image;
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = DefaultActiveState;

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

        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is not SCEObject)
            {
                throw new InvalidCContainerHolderException("CContainerHolder must be Object.");
            }
        }

        /// <summary>
        /// Creates a shallow copy of this instance.
        /// </summary>
        /// <remarks>
        /// <i>Note: Only the Image of this instance is cloned over.</i>
        /// </remarks>
        /// <returns>A shallow copy of this instance.</returns>
        public ImageComponent Clone()
        {
            return new(Name, Image.Clone());
        }

        /// <inheritdoc/>
        public Image GetImage()
        {
            return Image;
        }
    }
}
