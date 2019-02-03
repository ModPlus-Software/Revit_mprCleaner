namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllViews : WipeOption
    {
        readonly UIDocument _uiDoc;

        internal RemoveAllViews(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (любого типа, кроме открытых видов и листов)
            Name = Language.GetItem(RevitCommand.LangItem, "w39");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(View), 0, Name);
        }
    }
}
