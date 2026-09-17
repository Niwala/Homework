namespace SamsBackpack.Homework
{
    public enum Status
    {
        /// <summary>Treats it the same way as a hidden element by default</summary>
        Unchecked,
        Hidden,
        Available,
        /// <summary>Will be disable soon</summary>
        Limited,
        /// <summary>Read & Export only</summary>
        Locked,
    }
}