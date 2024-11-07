﻿namespace SCECorePlus.Components.RHS
{
    using SCEComponents;

    using SCECorePlus.Objects;

    // WorldSpace RenderHandlerComponent
    public class WorldSpaceRHC : IRenderable, IComponent
    {
        private const bool DefaultActiveState = true;

        private readonly List<ImageRenderPackage> renderList = new();

        private readonly List<Area2DInt> loadedAreaList = new();

        private readonly List<Camera> cameraList = new();

        private readonly List<Area2DInt> cameraWorldAlignedAreaCacheList = new();

        private CContainer? cContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="dimensions">The dimensions of the world space.</param>
        /// <param name="isActive">The initial active state of the world space.</param>
        public WorldSpaceRHC(string name, Vector2Int dimensions, bool isActive = DefaultActiveState)
        {
            Image = new(dimensions, isActive);

            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldSpaceRHC"/> class.
        /// </summary>
        /// <param name="dimensions">The dimensions of the world space.</param>
        /// <param name="bgColor">The background color of the world space.</param>
        /// <param name="isActive">The intial active state of the world space.</param>
        public WorldSpaceRHC(string name, Vector2Int dimensions, Color bgColor, bool isActive = DefaultActiveState)
        {
            Image = new(dimensions, bgColor, isActive);

            Name = name;
            BgColor = bgColor;
        }

        /// <summary>
        /// Gets the grid area of the world space.
        /// </summary>
        public Area2DInt GridArea { get => Image.GridArea; }

        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the world space is active.
        /// </summary>
        public bool IsActive
        {
            get => Image.IsActive;
            set => Image.IsActive = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector2Int"/> position of the world space.
        /// </summary>
        public Vector2Int Position
        {
            get => Image.Position;
            set => Image.Position = value;
        }

        /// <summary>
        /// Gets or sets the render layer of the world space.
        /// </summary>
        public byte Layer
        {
            get => Image.Layer;
            set => Image.Layer = value;
        }

        /// <summary>
        /// Gets or sets the background color of the world space.
        /// </summary>
        public Color BgColor { get; set; }

        /// <summary>
        /// Gets the camera list of the world space.
        /// </summary>
        public List<Camera> CameraList { get => cameraList; }

        /// <summary>
        /// Gets a value indicating whether the world space contains any camera.
        /// </summary>
        public bool HasCamera { get => CameraList.Count != 0; }

        /// <summary>
        /// Gets or sets a value indicating whether the world space should render when GameHandler calls OnUpdate, rather called through IRenderable.
        /// </summary>
        public bool RenderOnUpdate { get; set; } = false;

        /// <summary>
        /// Gets the current width of the world space.
        /// </summary>
        public int Width { get => Image.Width; }

        /// <summary>
        /// Gets the current height of the world space.
        /// </summary>
        public int Height { get => Image.Height; }

        /// <summary>
        /// Gets the current dimensions of the world space.
        /// </summary>
        public Vector2Int Dimensions { get => Image.Dimensions; }

        private Area2DInt CachedGridArea { get; set; }

        private Image Image { get; }

        private CContainer CContainer { get => cContainer ?? throw new NullReferenceException("CContainer is null."); }

        private World World { get => (World)CContainer.CContainerHolder; }

        private List<SCEObject> ObjectList { get => World.ObjectList; }

        /// <inheritdoc/>
        public Image GetImage()
        {
            if (!RenderOnUpdate)
            {
                Render();
            }

            return Image;
        }

        public void SetCContainer(CContainer? cContainer, ICContainerHolder holder)
        {
            if (holder is World)
            {
                this.cContainer = cContainer;
            }
            else
            {
                throw new InvalidCContainerHolderException("CContainerHolder is not World.");
            }
        }

        public void Update()
        {
            if (IsActive && RenderOnUpdate)
            {
                Render();
            }
        }

        /// <summary>
        /// Resizes the world space clearing all previous data.
        /// </summary>
        /// <param name="width">The new width of the world space.</param>
        /// <param name="height">The new height of the world space.</param>
        public void CleanResize(int width, int height)
        {
            Image.CleanResize(width, height);
            Image.BgColorFill(BgColor);
            loadedAreaList.Clear();
        }

        /// <summary>
        /// Resizes the world space clearing all previous data.
        /// </summary>
        /// <param name="dimensions">The new dimensions of the world space.</param>
        public void CleanResize(Vector2Int dimensions)
        {
            CleanResize(dimensions.X, dimensions.Y);
        }

        private void SmartClear()
        {
            Pixel clearPixel = new(BgColor);

            foreach (Area2DInt area in loadedAreaList)
            {
                Image.FillArea(clearPixel, area, true);
            }
                
            loadedAreaList.Clear();
        }

        private void Render()
        {
            SmartClear();

            UpdateRenderList();

            CacheGridArea();
            LoadRenderListToWorldSpace();
        }

        private void UpdateRenderList()
        {
            CacheCameraWorldAlignedAreas();

            RepopulateRenderList();

            SortRenderList();
        }

        private void RepopulateRenderList()
        {
            renderList.Clear();

            if (HasCamera && !AnyCachedCameraWorldAlignedAreaOverlapsWorldSpace())
            {
                return;
            }

            foreach (SCEObject obj in ObjectList)
            {
                if (obj.IsActive)
                {
                    TryAddRenderableComponents(obj);
                }
            }
        }

        private void TryAddRenderableComponents(SCEObject obj)
        {
            foreach (IComponent component in obj.CContainer)
            {
                if (component.IsActive && component is IRenderable renderable)
                {
                    Image image = renderable.GetImage();

                    TryAddToRenderList(image, obj.GridPosition);
                }
            }
        }

        private void TryAddToRenderList(Image image, Vector2Int offset)
        {
            if ((!HasCamera && Area2DInt.Overlaps(CachedGridArea, image.GridArea + offset)) || (HasCamera && AnyCachedCameraWorldAlignedAreaOverlapsArea(image.GridArea + offset)))
            {
                renderList.Add(new ImageRenderPackage(image, offset));
            }
        }

        private void SortRenderList()
        {
            bool swapped;
            do
            {
                swapped = false;
                for (int i = 1; i < renderList.Count; i++)
                {
                    ImageRenderPackage current = renderList[i], last = renderList[i - 1];

                    if (last.Image.Layer > current.Image.Layer)
                    {
                        (renderList[i], renderList[i - 1]) = (last, current);

                        swapped = true;
                    }
                }
            }
            while (swapped);
        }

        private void LoadRenderListToWorldSpace()
        {
            foreach (ImageRenderPackage renderPackage in renderList)
            {
                if (HasCamera)
                {
                    foreach (Camera camera in CameraList)
                    {
                        MapImageComponentWithinArea(renderPackage, camera.WorldAlignedArea);
                    }
                }
                else
                {
                    MapImageComponentWithinArea(renderPackage, CachedGridArea);
                }
            }
        }

        private void MapImageComponentWithinArea(ImageRenderPackage renderPackage, Area2DInt area)
        {
            Vector2Int alignedImagePosition = renderPackage.AlignedPosition;

            Area2DInt alignedResizedImageArea = area.TrimArea(renderPackage.AlignedArea);

            Area2DInt resizedImageGridArea = alignedResizedImageArea - alignedImagePosition;

            Image.MapToArea(renderPackage.Image, resizedImageGridArea, alignedImagePosition, true);

            loadedAreaList.Add(alignedResizedImageArea);
        }
        
        // Cache functions
        private void CacheGridArea()
        {
            CachedGridArea = Image.GridArea;
        }

        // Camera functions
        private void CacheCameraWorldAlignedAreas()
        {
            cameraWorldAlignedAreaCacheList.Clear();

            foreach (Camera camera in cameraList)
            {
                cameraWorldAlignedAreaCacheList.Add(camera.WorldAlignedArea);
            }
        }

        private bool AnyCachedCameraWorldAlignedAreaOverlapsImage(Image image, Vector2Int offset)
        {
            Vector2Int alignedPosStart = image.Position + offset;
            Vector2Int alignedPosEnd = alignedPosStart + image.Dimensions;

            foreach (Area2DInt worldAlignedArea in cameraWorldAlignedAreaCacheList)
            {
                worldAlignedArea.Expose(out Vector2Int start, out Vector2Int end);

                if (Area2DInt.Overlaps(alignedPosStart, alignedPosEnd, start, end))
                {
                    return true;
                }
            }

            return false;
        }

        private bool AnyCachedCameraWorldAlignedAreaOverlapsArea(Area2DInt alignedImageArea)
        {
            foreach (Area2DInt worldAlignedArea in cameraWorldAlignedAreaCacheList)
            {
                if (Area2DInt.Overlaps(worldAlignedArea, alignedImageArea))
                {
                    return true;
                }
            }

            return false;
        }

        private bool AnyCachedCameraWorldAlignedAreaOverlapsWorldSpace()
        {
            return AnyCachedCameraWorldAlignedAreaOverlapsArea(CachedGridArea);
        }
    }
}
