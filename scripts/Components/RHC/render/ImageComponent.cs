namespace SCECorePlus.Components.RHS
{
    using System.Diagnostics.CodeAnalysis;

    using SCECorePlus.Objects;
    using SCECore.ComponentSystem;

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
        public ImageComponent(bool isActive = DefaultActiveState)
        {
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageComponent"/> class.
        /// </summary>
        /// <param name="image">The initial image.</param>
        /// <param name="isActive">The initial active state of the image component.</param>
        public ImageComponent(Image image, bool isActive = DefaultActiveState)
            : this(isActive)
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

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the object aligned <see cref="Area2DInt"/> area of the object aligned position and the object aligned position corner.
        /// </summary>
        public Area2DInt ObjectAlignedArea { get => new(ObjectAlignedPosition, ObjectAlignedPositionCorner); }

        /// <summary>
        /// Gets the object aligned <see cref="Vector2Int"/> position of the image aligned position and the parent object position.
        /// </summary>
        public Vector2Int ObjectAlignedPosition { get => Image.AlignedPosition + (Vector2Int)Object.Position.Round(); }

        /// <summary>
        /// Gets the zero-based top-right corner position object aligned position.
        /// </summary>
        public Vector2Int ObjectAlignedPositionCorner { get => Image.AlignedPositionCorner + (Vector2Int)Object.Position.Round(); }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        /// <summary>
        /// Gets the parent Object of this instance.
        /// </summary>
        private SCEObject Object { get => (SCEObject)CContainer.CContainerHolder; }

        /// <summary>
        /// Creates a shallow copy of this instance.
        /// </summary>
        /// <remarks>
        /// <i>Note: Only the Image of this instance is cloned over.</i>
        /// </remarks>
        /// <returns>A shallow copy of this instance.</returns>
        public ImageComponent Clone()
        {
            return new(Image.Clone());
        }

        /// <inheritdoc/>
        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is SCEObject)
            {
                this.cContainer = cContainer;
            }
            else
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
