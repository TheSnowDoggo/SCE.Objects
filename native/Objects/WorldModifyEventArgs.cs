namespace SCECorePlus.Objects
{
    public class WorldModifyEventArgs : EventArgs
    {
        public WorldModifyEventArgs(SCEObject obj, ModifyType type)
        {
            Object = obj;
            Type = type;
        }

        public SCEObject Object { get; private set; }

        public ModifyType Type { get; private set; }

        public enum ModifyType : byte
        {
            Add = 0,
            Remove = 1,
            ModifyName = 2,
            ModifyPosition = 3,
            ModifyIsActive = 4,
        }
    }
}
