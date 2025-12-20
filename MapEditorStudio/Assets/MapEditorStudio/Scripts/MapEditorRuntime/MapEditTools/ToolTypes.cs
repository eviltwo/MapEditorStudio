namespace MapEditorStudio.MapEditor
{
    public enum ToolTypes
    {
        Create = 0,
        Move = 1,
        Delete = 2,

        EnumCount,
    }

    public static class ToolTypesUtility
    {
        public static int ToolCount => (int)ToolTypes.EnumCount;

        public static ToolTypes GetNext(ToolTypes current) => (ToolTypes)(((int)current + 1) % ToolCount);

        public static ToolTypes GetPrevious(ToolTypes current) => (ToolTypes)(((int)current + (int)ToolTypes.EnumCount - 1) % ToolCount);
    }
}
