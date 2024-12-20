namespace SCE
{
    public class ChunkTemplate
    {
        public ChunkTemplate(int width, int height)
        {
            Width = width;
            Height = height;
            Dimensions = new(width, height);
        }

        public ChunkTemplate(Vector2Int dimensions)
        {
            Width = dimensions.X;
            Height = dimensions.Y;
            Dimensions = dimensions;
        }

        public int Width { get; }

        public int Height { get; }

        public Vector2Int Dimensions { get; }
    }
}
