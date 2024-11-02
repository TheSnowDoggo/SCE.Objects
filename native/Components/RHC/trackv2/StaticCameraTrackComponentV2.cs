namespace SCECorePlus.Components.RHS
{
    using SCEComponents;

    using SCECorePlus.Objects;
    using SCECorePlus.Types;

    /// <summary>
    /// An <see cref="IComponent"/> used for static object camrea tracking.
    /// </summary>
    public class StaticCameraTrackComponentV2 : IComponent
    {
        private const bool DefaultActiveState = true;

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticCameraTrackComponentV2"/> class.
        /// </summary>
        /// <param name="obj">The object to track.</param>
        /// <param name="anchor">The position anchor of the camera.</param>
        /// <param name="isActive">The active state of the component.</param>
        public StaticCameraTrackComponentV2(string name, SCEObject obj, Anchor anchor, bool isActive = DefaultActiveState)
        {
            Name = name;
            Object = obj;
            Anchor = anchor;
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticCameraTrackComponentV2"/> class.
        /// </summary>
        /// <param name="obj">The object to track.</param>
        /// <param name="isActive">The active state of the component.</param>
        public StaticCameraTrackComponentV2(string name, SCEObject obj, bool isActive = DefaultActiveState)
            : this(name, obj, new Anchor(), isActive)
        {
        }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the controlled camera.
        /// </summary>
        public SCEObject Object { get; set; }

        /// <summary>
        /// Gets or sets the position anchor of the controlled camera.
        /// </summary>
        public Anchor Anchor { get; set; }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private CameraV2 Camera { get => (CameraV2)CContainer.CContainerHolder; }

        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is CameraV2)
            {
                this.cContainer = cContainer;
            }
            else
            {
                throw new ArgumentException("CContainerHolder is not CameraV2.");
            }
        }

        /// <inheritdoc/>
        public void Update()
        {
            Camera.WorldPosition = Object.Position + (Vector2)Anchor.GetAlignedOffset(Camera.Dimensions);
        }
    }
}
