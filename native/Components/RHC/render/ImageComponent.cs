namespace SCECorePlus.Components.RHS
{
    using System.Diagnostics.CodeAnalysis;

    using SCEComponents;

    using SCECorePlus.Objects;

    /// <summary>
    /// An <see cref="IComponent"/> used for storing an image in an object.
    /// </summary>
    public class ImageComponent : IComponent, IRenderable
    {
        private const bool DefaultActiveState = true;

        private CContainer? cContainer;

        private Image? image;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageComponent"/> class.
        /// </summary>
        /// <param name="isActive">The initial active state of the image component.</param>
        public ImageComponent(string name, bool isActive = DefaultActiveState)
        {
            Name = name;
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageComponent"/> class.
        /// </summary>
        /// <param name="image">The initial image.</param>
        /// <param name="isActive">The initial active state of the image component.</param>
        public ImageComponent(string name, Image image, bool isActive = DefaultActiveState)
            : this(name, isActive)
        {
            Image = image;
        }

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

        public string Name { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        public event EventHandler? ComponentModifyEvent;

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
        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is not SCEObject)
            {
                throw new InvalidCContainerHolderException("CContainerHolder must be Object.");
            }
        }

        /// <inheritdoc/>
        public Image GetImage()
        {
            return Image;
        }
    }
}
