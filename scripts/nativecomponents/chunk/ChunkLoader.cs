namespace SCE
{
    public class ChunkLoader
    {
        private readonly Vector2Int chunkDimensions = new(16, 16);

        public Vector2Int GetChunkPosition(Vector2Int position)
        {
            return position / chunkDimensions;
        }

        public Rect2D GetChunkArea(Rect2D area)
        {
            return new(GetChunkPosition(area.Start()), GetChunkPosition(area.End()));
        }

        public Vector2Int[] ResolveChunks(Rect2D area)
        {
            Rect2D chunkArea = GetChunkArea(area);

            Vector2Int[] chunkPosArr = new Vector2Int[area.Size()];
            int i = 0;
            for (int x = chunkArea.Left; x < chunkArea.Right; ++x)
            {
                for (int y = chunkArea.Bottom; y < chunkArea.Top; ++y)
                {
                    chunkPosArr[i] = new Vector2Int(x, y);
                    ++i;
                }
            }
            return chunkPosArr;
        }
    }
}
 