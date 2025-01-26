namespace SCE
{
    public interface IRenderRule : ISearcheable
    {
        bool IsActive { get; set; }
        bool ShouldRender(SCEObject obj);
    }
}
