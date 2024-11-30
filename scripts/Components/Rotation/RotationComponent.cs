namespace SCECorePlus
{
    using SCEComponents;

    /// <summary>
    /// An <see cref="LunaSCE.Image"/> <see cref="IComponent"/> class for simualting rotation through <see cref="Vector2"/> position transformation.
    /// </summary>
    public class RotationComponent : IComponent
    {
        private const bool DefaultActiveState = true;

        private CContainer? cContainer;

        private int rotationFactor;

        private AlignType rotationalAlignment;

        private Vector2Int offsetPosition;

        private Vector2? rotationAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationComponent"/> class.
        /// </summary>
        /// <param name="isActive">The starting active status of the <see cref="RotationComponent"/>.</param>
        public RotationComponent(string name, bool isActive = DefaultActiveState)
        {
            Name = name;
            IsActive = isActive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationComponent"/> class.
        /// </summary>
        /// <param name="rotationalAlignment">The starting rotational alignment mode.</param>
        /// <param name="isActive">The starting active status of the <see cref="RotationComponent"/>.</param>
        public RotationComponent(string name, AlignType rotationalAlignment, bool isActive = DefaultActiveState)
            : this(name, isActive)
        {
            RotationalAlignment = rotationalAlignment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationComponent"/> class.
        /// </summary>
        /// <param name="rotationalAlignment">The starting rotational alignment mode.</param>
        /// <param name="customRotationAxis">The <see cref="Vector2"/> rotation axis used when <see cref="AlignType.Custom"/> is set.</param>
        /// <param name="isActive">The starting active status of the <see cref="RotationComponent"/>.</param>
        public RotationComponent(string name, AlignType rotationalAlignment, Vector2 customRotationAxis, bool isActive = DefaultActiveState)
            : this(name, rotationalAlignment, isActive)
        {
            CustomRotationAxis = customRotationAxis;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationComponent"/> class.
        /// </summary>
        /// <param name="rotationalAlignment">The starting rotational alignment mode.</param>
        /// <param name="customRotationAxis">The <see cref="Vector2"/> rotation axis used when <see cref="AlignType.Custom"/> is set.</param>
        /// <param name="rotationFactor">The starting rotation factor.</param>
        /// <param name="offsetPosition">The starting <see cref="Vector2Int"/> offset position.</param>
        /// <param name="isActive">The starting active status of the <see cref="RotationComponent"/>.</param>
        public RotationComponent(string name, AlignType rotationalAlignment, Vector2 customRotationAxis, int rotationFactor, Vector2Int offsetPosition, bool isActive = DefaultActiveState)
            : this(name, rotationalAlignment, customRotationAxis, isActive)
        {
            RotationFactor = rotationFactor;
            this.offsetPosition = offsetPosition;
        }

        public string Name { get; set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        public event EventHandler? ComponentModifyEvent;

        /// <summary>
        /// Gets the rotationally aligned <see cref="Area2DInt"/> of the associated <see cref="LunaSCE.Image"/>.
        /// </summary>
        public Area2DInt AlignedArea => new(AlignedPosition, AlignedPositionCorner);

        /// <summary>
        /// Gets the rotationally aligned <see cref="Vector2"/> position of the associated <see cref="LunaSCE.Image"/>.
        /// </summary>
        public Vector2Int AlignedPosition => Image.Position + offsetPosition - GetCornerOffset();

        /// <summary>
        /// Gets the zero-based top-right corner position from the rotationally aligned <see cref="Vector2"/> position of the associated <see cref="LunaSCE.Image"/>.
        /// </summary>
        public Vector2Int AlignedPositionCorner => AlignedPosition + Image.Dimensions;

        /// <summary>
        /// Gets or sets the <see cref="Vector2"/> rotation axis used when <see cref="AlignType.Custom"/> is set.
        /// </summary>
        public Vector2 CustomRotationAxis { get; set; }

        /// <summary>
        /// Gets or sets the current rotational <see cref="AlignType"/> which determines the axis to rotate about.
        /// </summary>
        public AlignType RotationalAlignment
        {
            get => rotationalAlignment;
            set
            {
                rotationalAlignment = value;
                rotationAxis = null;
            }
        }

        /// <summary>
        /// Gets the current rotation factor.
        /// </summary>
        public int RotationFactor
        {
            get => rotationFactor;
            private set
            {
                if (!RotationUtils.RotationRange.InRange(value))
                {
                    throw new ArgumentException("Rotation factor is not within the excepted range.");
                }

                rotationFactor = value;
            }
        }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException(); }

        private Image Image => (Image)CContainer.CContainerHolder;

        /// <summary>
        /// Gets the rotation axis.
        /// </summary>
        private Vector2 RotationAxis => rotationAxis ??= GetRotationAxis();

        /// <inheritdoc/>
        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is Image)
            {
                this.cContainer = cContainer;

                Image.OnResize += Rotation_ImageOnResize;
            }
            else
            {
                throw new InvalidCContainerHolderException("CContainerHolder must be Image or null");
            }
        }

        /// <summary>
        /// Resets the current center midpoint.
        /// </summary>
        public void ResetCenterMidpoint()
        {
            rotationAxis = null;
        }

        /// <summary>
        /// Rotates the <see cref="Grid2D{T}"/> clockwise by 90 degrees.
        /// </summary>
        public void Rotate90Clockwise() => Rotate90(1);

        /// <summary>
        /// Rotates the <see cref="Grid2D{T}"/> anticlockwise by 90 degrees.
        /// </summary>
        public void Rotate90Anticlockwise() => Rotate90(-1);

        /// <summary>
        /// Rotates the <see cref="Grid2D{T}"/> by 90 degrees given the direction to rotate.
        /// </summary>
        /// <param name="direction">The direction to rotate in. Must be either 1 (clockwise) or -1 (anticlockwise).</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="direction"/> is not either 1 or -1.</exception>
        public void Rotate90(int direction)
        {
            // Must happen before image rotation as it can mess up the center midpoint.
            Vector2 rotationAxis = RotationAxis;

            Image.RotateData90(direction);

            Vector2 rotatedOffsetPosition = RotationUtils.GetRotatedOffsetPosition(offsetPosition, direction, rotationAxis);

            offsetPosition = (rotatedOffsetPosition + rotationAxis).ToVector2Int();

            RotationFactor = RotationUtils.GetNewRotation(RotationFactor, direction);
        }

        /// <summary>
        /// Resets stored rotation data.
        /// </summary>
        public void ResetRotationData()
        {
            offsetPosition = Vector2Int.Zero;
            RotationFactor = 0;
            rotationAxis = null;
        }

        /// <summary>
        /// Represents the rotational alignment mode which determines the rotation axis to rotate about.
        /// </summary>
        public enum AlignType
        {
            /// <summary>
            /// Rotates about the given custom <see cref="Vector2"/> rotation axis.
            /// </summary>
            Custom,

            /// <summary>
            /// Rotates about the starting midpoint of the <see cref="Image"/>.
            /// </summary>
            Center,

            /// <summary>
            /// Rotates about the bottom-left of the <see cref="Image"/>.
            /// </summary>
            BottomLeft,

            /// <summary>
            /// Rotates about the bottom-right of the <see cref="Image"/>.
            /// </summary>
            BottomRight,

            /// <summary>
            /// Rotates about the top-left of the <see cref="Image"/>.
            /// </summary>
            TopLeft,

            /// <summary>
            /// Rotates about the top-right of the <see cref="Image"/>.
            /// </summary>
            TopRight,
        }

        /// <summary>
        /// Returns the <see cref="Vector2Int"/> corner offset to offset the aligned position by.
        /// </summary>
        /// <returns>The <see cref="Vector2Int"/> corner offset to offset the aligned position by.</returns>
        private Vector2Int GetCornerOffset()
        {
            return RotationFactor switch
            {
                0 => Vector2Int.Zero,
                1 => new(0, Image.Dimensions.Y - 1),
                2 => Image.Dimensions - 1,
                3 => new(Image.Dimensions.X - 1, 0),
                _ => throw new ArgumentException("Current rotation is invalid.")
            };
        }

        /// <summary>
        /// Returns the <see cref="Vector2"/> rotation axis to rotate about given the current <see cref="RotationalAlignment"/>.
        /// </summary>
        /// <returns>The <see cref="Vector2"/> rotation axis to rotate about.</returns>
        private Vector2 GetRotationAxis()
        {
            return RotationalAlignment switch
            {
                AlignType.Custom => CustomRotationAxis,
                AlignType.Center => Image.Dimensions.ToVector2().Midpoint,
                AlignType.BottomLeft => Vector2.Zero,
                AlignType.BottomRight => new(Image.Dimensions.X - 1, 0),
                AlignType.TopLeft => new(0, Image.Dimensions.Y - 1),
                AlignType.TopRight => Image.Dimensions.ToVector2() - 1,
                _ => throw new NotImplementedException("Rotational alignment AlignType is unknown.")
            };
        }

        private void Rotation_ImageOnResize()
        {
            ResetRotationData();
        }
    }
}
