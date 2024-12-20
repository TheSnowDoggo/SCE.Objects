namespace SCE
{
    public class ChunkComponent<T> : ComponentBase<SCEObject>
    {
        public ChunkComponent(ChunkTemplate chunkTemplate, Grid2D<T> grid)
            : base()
        {
            Grid = grid;
        }

        public Vector2Int ChunkPosition { get; set; }

        public Grid2D<T> Grid { get; set; }
    }
}
