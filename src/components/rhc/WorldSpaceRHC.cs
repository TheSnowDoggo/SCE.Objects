namespace SCE
{
    // WorldSpace RenderHandlerComponentV2
    public class WorldSpaceRHC : ComponentBase<World>, ICContainerHolder, IUpdate
    {
        public WorldSpaceRHC(params IComponent[] arr)
        {
            Components = new(this, arr);
        }

        public SCEColor BgColor { get; set; }

        public IUpdateLimit? RenderLimiter { get; set; }

        public CContainer Components { get; }

        /// <inheritdoc/>
        public void Update()
        {
            if (!RenderLimiter?.OnUpdate() ?? false || !Components.Contains<Camera>())
            {
                return;
            }

            Components.Update();

            Dictionary<Camera, List<SpritePackage>> cameras = new();

            foreach (var camera in Components.EnumerateType<Camera>())
            {
                if (camera.IsActive)
                {
                    cameras[camera] = new();
                }
            }

            List<SpritePackage> irpList = new();

            foreach ((var obj, var r) in EnumerateRenderables())
            {
                var dpMap = r.GetMapView();

                var dpStart = AnchorUtils.DimensionFix(r.Anchor, dpMap.Dimensions) - dpMap.Dimensions + r.Offset;

                var start = (Vector2Int)((dpStart / new Vector2(2, 1) + obj.GlobalPosition) * new Vector2(2, 1)).Round();

                irpList.Add(new SpritePackage(dpMap, r.Layer, start));
            }

            foreach ((var camera, var packages) in cameras)
            {
                var renderArea = camera.RenderArea();
                foreach (var irp in irpList)
                {
                    if (Rect2DInt.Overlaps(renderArea, irp.OffsetArea))
                    {
                        packages.Add(irp);
                    }
                }
            }

            foreach ((var camera, var packages) in cameras)
            {
                camera.Render(packages);
            }
        }

        private IEnumerable<(SCEObject, IRenderable)> EnumerateRenderables()
        {
            foreach (var obj in Holder.EnumerateActive())
            {
                foreach (var r in obj.Components.EnumerateType<IRenderable>())
                {
                    if (r.IsActive)
                    {
                        yield return (obj, r);
                    }
                }
            }
        }
    }
}
