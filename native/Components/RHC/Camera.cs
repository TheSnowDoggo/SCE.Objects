namespace SCECorePlus.Components.RHS
{
    using SCECore.ComponentSystem;

    /// <summary>
    /// Represents a camera in a world space.
    /// </summary>
    public class Camera : Image, IRenderable
    {
        private const bool DefaultActiveState = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="dimensions">The dimensions of the camera.</param>
        /// <param name="cList">The initial cList of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, CList cList, bool isActive = DefaultActiveState)
            : base(dimensions, isActive)
        {
            WorldSpace = worldSpace;

            CContainer = new(this, cList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="dimensions">The dimensions of the camera.</param>
        /// <param name="bgColor">The background color of the camera.</param>
        /// <param name="cList">The initial cList of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, Color bgColor, CList cList, bool isActive = DefaultActiveState)
            : base(dimensions, bgColor, isActive)
        {
            WorldSpace = worldSpace;
            BgColor = bgColor;

            CContainer = new(this, cList);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="dimensions">The dimensions of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, bool isActive = DefaultActiveState)
            : this(worldSpace, dimensions, new CList(), isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="dimensions">The dimensions of the camera.</param>
        /// <param name="bgColor">The background color of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public Camera(WorldSpaceRHC worldSpace, Vector2Int dimensions, Color bgColor, bool isActive = DefaultActiveState)
            : this(worldSpace, dimensions, bgColor, new CList(), isActive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="image">The base image of the camera.</param>
        /// <param name="worldSpace">The WorldSpace to render from.</param>
        /// <param name="bgColor">The background color of the camera.</param>
        /// <param name="isActive">The active render status of the camera.</param>
        public Camera(Image image, WorldSpaceRHC worldSpace, Color bgColor, CList cList, bool isActive = DefaultActiveState)
            : base(image)
        {
            WorldSpace = worldSpace;
            BgColor = bgColor;
            IsActive = isActive;
            CContainer = new(this, cList);
        }

        /// <summary>
        /// Gets or sets the delegate called after rendering WorldSpace and before returning itself in GetImage (IRenderable)
        /// </summary>
        public CallOnRender? AfterRender { get; set; }

        /// <summary>
        /// Gets the GridArea of this instance aligned to its WorldPosition.
        /// </summary>
        public Area2DInt WorldAlignedArea => GridArea + WorldPositionInt;

        /// <summary>
        /// Gets or sets the position of this instance in the WorldSpace.
        /// </summary>
        public Vector2 WorldPosition { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets the world position rounded and converted to the nearest <see cref="Vector2Int"/>.
        /// </summary>
        public Vector2Int WorldPositionInt => (Vector2Int)WorldPosition.Round();

        /// <summary>
        /// Gets or sets the WorldSpace of this instance to render from.
        /// </summary>
        public WorldSpaceRHC WorldSpace { get; set; }

        /// <summary>
        /// Gets or sets the background color of this instance.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera should update every component on render.
        /// </summary>
        public bool UpdateOnRender { get; set; } = false;

        /// <inheritdoc/>
        public override Image GetImage()
        {
            if (UpdateOnRender)
            {
                CContainer.Update();
            }

            OnRender?.Invoke();
            Render();
            AfterRender?.Invoke();

            return this;
        }

        /// <summary>
        /// Renders this instance from WorldSpace.
        /// </summary>
        private void Render()
        {
            Image worldImage = WorldSpace.GetImage();

            if (WorldAlignedArea.OverlapsWith(worldImage.GridArea))
            {
                Area2DInt fixedGridArea = worldImage.GridArea.TrimArea(WorldAlignedArea, out bool hasFixed) - WorldPositionInt;

                if (hasFixed)
                {
                    FillBackground();
                }

                MapAreaFrom(worldImage, fixedGridArea, WorldPositionInt);
            }
            else
            {
                FillBackground();
            }

            OnRender?.Invoke();
        }

        /// <inheritdoc cref="DisplayMap.Fill"/>
        private void FillBackground() => Fill(new Pixel(BgColor));
    }
}
