namespace SCE
{
    /// <summary>
    /// An <see cref="IComponent"/> used for static object camrea tracking.
    /// </summary>
    public class StaticCameraTrackComponent : IComponent
    {
        private const bool DefaultActiveState = true;

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticCameraTrackComponent"/> class.
        /// </summary>
        /// <param name="obj">The object to track.</param>
        public StaticCameraTrackComponent(string name, SCEObject obj)
        {
            Name = name;
            Object = obj;
        }

        public string Name { get; set; }

        public bool IsActive { get; set; } = DefaultActiveState;

        /// <summary>
        /// Gets or sets the controlled camera.
        /// </summary>
        public SCEObject Object { get; set; }

        /// <summary>
        /// Gets or sets the position anchor of the controlled camera.
        /// </summary>
        public Anchor Anchor { get; set; }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private Camera Camera { get => (Camera)CContainer.CContainerHolder; }

        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is Camera)
            {
                this.cContainer = cContainer;
            }
            else
            {
                throw new ArgumentException("CContainerHolder is not Camera.");
            }
        }

        /// <inheritdoc/>
        public void Update()
        {
            Camera.WorldPosition = Object.Position + (Vector2)Anchor.GetAlignedOffset(Camera.Dimensions);
        }
    }
}
