namespace SCE
{
    public static class ObjectExtensions
    {
        public static Vector2Int GridPosition(this IObject obj)
        {
            return (Vector2Int)obj.Position.Round();
        }
    }
}
