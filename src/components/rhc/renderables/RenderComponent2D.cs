﻿namespace SCE
{
    /// <summary>
    /// An <see cref="IComponent"/> used for storing an <see cref="IRenderable"/> in an object.
    /// </summary>
    public class RenderComponent2D : ComponentBase<SCEObject>, IRenderable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent2D"/> class.
        /// </summary>
        /// <param name="renderable">The initial <see cref="IRenderable"/>.</param>
        public RenderComponent2D(IRenderable renderable)
            : base()
        {
            Renderable = renderable;
        }

        public IRenderable Renderable { get; set; }

        public Vector2Int Offset { get => Renderable.Offset; }

        public int Layer { get => Renderable.Layer; }

        public Anchor Anchor { get => Renderable.Anchor; }

        public DisplayMapView GetMapView()
        {
            return Renderable.GetMapView();
        }
    }
}
