namespace SCE
{
    /// <summary>
    /// An <see cref="IComponent"/> used for zone-based object camera tracking.
    /// </summary>
    public class ZoneCameraTrackComponent : ComponentBase<Camera>, IUpdate
    {
        private Vector2Int zoneDimensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneCameraTrackComponent"/> class.
        /// </summary>
        /// <param name="obj">The objcet to track.</param>
        /// <param name="boundingArea">The area the zone is bounded to.</param>
        /// <param name="zoneDimensions">The dimensions of the zone.</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="zoneDimensions"/> are invalid.</exception>
        public ZoneCameraTrackComponent(string name, SCEObject obj, Area2DInt boundingArea, Vector2Int zoneDimensions)
            : base(name)
        {
            if (zoneDimensions <= 0)
                throw new ArgumentException("Zone dimensions must be greater than zero.");

            Object = obj;

            BoundingArea = boundingArea;

            ZoneDimensions = zoneDimensions;
        }

        public ZoneCameraTrackComponent(SCEObject obj, Area2DInt boundingArea, Vector2Int zoneDimensions)
            : this("zone_camera_track", obj, boundingArea, zoneDimensions)
        {
        }

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
                    throw new ArgumentException("Zone dimensions must be greater than 0.");
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

        public void Update()
        {
            Parent.WorldPosition = (Vector2)(BoundObjectAlignedZonePosition + ZoneDimensions.Midpoint + -AnchorUtils.AnchoredDimension(CameraAnchor, Parent.Dimensions) + CameraPosition);
        }
    }
}
