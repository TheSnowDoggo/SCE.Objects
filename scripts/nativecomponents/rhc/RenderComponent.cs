namespace SCE
{
    /// <summary>
    /// An <see cref="IComponent"/> used for storing an <see cref="IRenderable"/> in an object.
    /// </summary>
    public class RenderComponent : ComponentBase<SCEObject>, IRenderable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        /// <param name="renderable">The initial <see cref="IRenderable"/>.</param>
        public RenderComponent(IRenderable? renderable = null)
            : base()
        {
            Renderable = renderable;
        }

        public IRenderable? Renderable { get; set; }

        private IRenderable ValidRenderable { get => Renderable ?? throw new NullReferenceException("Renderable is null."); }

        /// <summary>
        /// Gets a value indicating whether this instance has an image.
        /// </summary>
        public bool HasRenderable { get => Renderable != null; }

        public Vector2Int Position { get => ValidRenderable.Position; }

        public int Layer { get => ValidRenderable.Layer; }

        public Anchor Anchor { get => ValidRenderable.Anchor; }

        public DisplayMap GetMap()
        {
            return ValidRenderable.GetMap();
        }
    }
}
