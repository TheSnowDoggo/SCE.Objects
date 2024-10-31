namespace SCECorePlus.Components.RHS
{
    using System.Diagnostics.CodeAnalysis;

    using SCECore.ComponentSystem;

    using SCECorePlus.Objects;

    /// <summary>
    /// An <see cref="IComponent"/> used for storing an <see cref="IRenderable"/> in an object.
    /// </summary>
    public class RenderComponent : IComponent, IRenderable
    {
        private const bool DefaultActiveState = true;

        private IRenderable? renderable;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        /// <param name="isActive">The initial active state of the render component.</param>
        public RenderComponent(bool isActive = DefaultActiveState)
        {
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        /// <param name="renderable">The initial <see cref="IRenderable"/>.</param>
        /// <param name="isActive">The initial active state of the render component.</param>
        public RenderComponent(IRenderable renderable, bool isActive = DefaultActiveState)
            : this(isActive)
        {
            Renderable = renderable;
        }

        /// <summary>
        /// Gets or sets the stored Image of this instance.
        /// </summary>
        /// <remarks>
        /// <i>Note: The image may be null resulting in an exception being thrown.</i>
        /// </remarks>
        [AllowNull]
        public IRenderable Renderable
        {
            get => renderable ?? throw new NullReferenceException("Renderable is null.");
            set => renderable = value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has an image.
        /// </summary>
        public bool HasRenderable { get => renderable != null; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

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
            return Renderable.GetImage();
        }
    }
}
