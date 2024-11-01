namespace SCECorePlus.Objects
{
    public class ObjectModifyEventArgs : EventArgs
    {
        public ObjectModifyEventArgs(ModifyType type)
        {
            Type = type;
        }

        public ModifyType Type { get; private set; }

        public enum ModifyType : byte
        {
            Name = 0,
            Position = 1,
            IsActive = 2,
        }
    }
}
