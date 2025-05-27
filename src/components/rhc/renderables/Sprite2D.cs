namespace SCE
{
    public class Sprite2D : ComponentBase<SCEObject>, IRenderable, ISmartLayerable
    {
        public Sprite2D(DisplayMap dpMap)
            : base()
        {
            DisplayMap = dpMap;
        }

        public DisplayMap DisplayMap { get; set; }

        public Vector2Int Offset { get; set; }

        public int Layer { get; set; }

        public Anchor Anchor { get; set; }

        /// <inheritdoc/>
        public DisplayMapView GetMapView()
        {
            return (DisplayMapView)DisplayMap;
        }
    }
}
