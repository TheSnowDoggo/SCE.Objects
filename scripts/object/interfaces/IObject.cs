namespace SCE
{
    public interface IObject : IScene, ICContainerHolder, ISearcheable
    {
        Vector2 Position { get; set; }

        void SetWorld(World? world);
    }
}
