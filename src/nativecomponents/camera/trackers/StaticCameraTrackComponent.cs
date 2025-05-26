namespace SCE
{
    /// <summary>
    /// An <see cref="IComponent"/> used for static object camrea tracking.
    /// </summary>
    public class StaticCameraTrackComponent : ComponentBase<Camera>, IUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticCameraTrackComponent"/> class.
        /// </summary>
        /// <param name="obj">The object to track.</param>
        public StaticCameraTrackComponent(SCEObject obj)
            : base()
        {
            Object = obj;
        }

        /// <summary>
        /// Gets or sets the controlled camera.
        /// </summary>
        public SCEObject Object { get; set; }

        public Vector2Int Offset { get; set; }

        /// <summary>
        /// Gets or sets the position anchor of the controlled camera.
        /// </summary>
        public Anchor Anchor { get; set; }

        /// <inheritdoc/>
        public void Update()
        {
            Holder.WorldPosition = Object.WorldPosition -(Vector2)AnchorUtils.DimensionFix(Anchor, Holder.Dimensions / new Vector2Int(2, 1)) + (Vector2)Offset;
        }
    }
}
