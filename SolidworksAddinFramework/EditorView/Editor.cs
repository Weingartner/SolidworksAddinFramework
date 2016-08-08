namespace SolidworksAddinFramework.EditorView
{
    public static class Editor
    {
        public static IEditor Closed => new EditorEmpty();
    }
}