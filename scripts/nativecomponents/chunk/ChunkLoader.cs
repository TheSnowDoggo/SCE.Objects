namespace SCE
{
    public class ChunkLoader
    {
        private readonly Vector2Int chunkDimensions = new(16, 16);

        public Vector2Int GetChunkPosition(Vector2Int position)
        {
            return position / chunkDimensions;
        }

        public Area2DInt GetChunkArea(Area2DInt area)
        {
            return new(GetChunkPosition(area.Start), GetChunkPosition(area.End));
        }

        public Vector2Int[] ResolveChunks(Area2DInt area)
        {
            Area2DInt chunkArea = GetChunkArea(area);

            Vector2Int[] chunkPosArr = new Vector2Int[area.Dimensions.ScalarProduct];
            int i = 0;
            for (int x = chunkArea.Start.X; x < chunkArea.End.X; ++x)
            {
                for (int y = chunkArea.Start.Y; y < chunkArea.End.Y; ++y)
                {
                    chunkPosArr[i] = new Vector2Int(x, y);
                    ++i;
                }
            }
            return chunkPosArr;
        }
    }
}
 