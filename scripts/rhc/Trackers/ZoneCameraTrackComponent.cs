namespace SCE
{
    /// <summary>
    /// An <see cref="IComponent"/> used for zone-based object camera tracking.
    /// </summary>
    public class ZoneCameraTrackComponent : IComponent
    {
        private const bool DefaultActiveState = true;

        private CContainer? cContainer;

        private Vector2Int zoneDimensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneCameraTrackComponent"/> class.
        /// </summary>
        /// <param name="obj">The objcet to track.</param>
        /// <param name="boundingArea">The area the zone is bounded to.</param>
        /// <param name="zoneDimensions">The dimensions of the zone.</param>
        /// <param name="zoneAnchor">The anchor of the zone.</param>
        /// <param name="cameraAnchor">The anchor of the camera.</param>
        /// <param name="isActive">The active state of the component.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="zoneDimensions"/> are invalid.</exception>
        public ZoneCameraTrackComponent(string name, SCEObject obj, Area2DInt boundingArea, Vector2Int zoneDimensions, bool isActive = DefaultActiveState)
        {
            if (zoneDimensions <= 0)
            {
                throw new ArgumentException("Zone dimensions must be greater than zero.");
            }

            Name = name;

            Object = obj;

            BoundingArea = boundingArea;

            ZoneDimensions = zoneDimensions;

            IsActive = isActive;
        }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the object to track.
        /// </summary>
        public SCEObject Object { get; set; }

        /// <summary>
        /// Gets or sets the camera bounding area.
        /// </summary>
        public Area2DInt BoundingArea { get; set; }

        /// <summary>
        /// Gets or sets the dimensions of the camera zone.
        /// </summary>
        public Vector2Int ZoneDimensions
        {
            get => zoneDimensions;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Zone dimensions must be greater than 0.");
                }

                zoneDimensions = value;
            }
        }

        public Vector2Int ZonePosition { get; set; }

        /// <summary>
        /// Gets or sets the anchor of the zone.
        /// </summary>
        public Anchor ZoneAnchor { get; set; }

        public Vector2Int CameraPosition { get; set; }

        /// <summary>
        /// Gets or sets the anchor of the camera.
        /// </summary>
        public Anchor CameraAnchor { get; set; }

        private Vector2Int ObjectAlignedZonePosition => Object.GridPosition + -AnchorUtils.AnchoredDimension(ZoneAnchor, ZoneDimensions) + ZonePosition;

        private Area2DInt BoundObjectAlignedZoneArea => BoundingArea.Bound(ObjectAlignedZoneArea);

        private Vector2Int BoundObjectAlignedZonePosition => BoundObjectAlignedZoneArea.Start;

        private Area2DInt ObjectAlignedZoneArea => new Area2DInt(Vector2Int.Zero, ZoneDimensions) + ObjectAlignedZonePosition;

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private Camera Camera { get => (Camera)CContainer.CContainerHolder; }

        public void SetCContainer(CContainer? cContainer, ICContainerHolder cContainerHolder)
        {
            if (cContainerHolder is Camera)
                this.cContainer = cContainer;
            else
                throw new InvalidCContainerHolderException("CContainerHolder is not Camera.");
        }

        public void Update()
        {
            Camera.WorldPosition = (Vector2)(BoundObjectAlignedZonePosition + ZoneDimensions.Midpoint + -AnchorUtils.AnchoredDimension(CameraAnchor, Camera.Dimensions) + CameraPosition);
        }
    }
}
