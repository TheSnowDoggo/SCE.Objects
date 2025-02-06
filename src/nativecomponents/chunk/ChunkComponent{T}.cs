namespace SCE
{
    public class ChunkComponent<T> : ComponentBase<SCEObject>
    {
        public ChunkComponent(string name, ChunkTemplate chunkTemplate, Grid2D<T> grid)
            : base(name)
        {
            Grid = grid;
        }

        public ChunkComponent(ChunkTemplate chunkTemplate, Grid2D<T> grid)
            : this("chunk", chunkTemplate, grid)
        {
        }

        public Vector2Int ChunkPosition { get; set; }

        public Grid2D<T> Grid { get; set; }
    }
}
