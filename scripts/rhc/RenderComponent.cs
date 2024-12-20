namespace SCE
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An <see cref="IComponent"/> used for storing an <see cref="IRenderable"/> in an object.
    /// </summary>
    public class RenderComponent : ComponentBase<SCEObject>, IRenderable
    {
        private IRenderable? renderable;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        public RenderComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        /// <param name="renderable">The initial <see cref="IRenderable"/>.</param>
        public RenderComponent(IRenderable renderable)
            : base()
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

        public Vector2Int Position { get => Renderable.Position; }

        public int Layer { get => Renderable.Layer; }

        public Anchor Anchor { get => Renderable.Anchor; }

        public DisplayMap GetMap()
        {
            return Renderable.GetMap();
        }
    }
}
