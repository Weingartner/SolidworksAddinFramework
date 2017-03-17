using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidworksAddinFramework
{
    public static class SelectionBoxExtensions
    {
        public static void SetSelectionColor(this IPropertyManagerPageSelectionbox box, swUserPreferenceIntegerValue_e color)
        {
            box.SetSelectionColor(true, (int)color);
        }
    }
}
