namespace SCE
{
    internal abstract class Collider
    {
        public LInfo LayerInfo { get; set; }

        public abstract bool CollidesWith();
    }
}
