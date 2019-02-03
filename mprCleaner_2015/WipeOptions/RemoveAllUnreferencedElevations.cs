namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedElevations : WipeOption
    {
        readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedElevations(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только фасады)
            Name = Language.GetItem(RevitCommand.LangItem, "w32");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewSection), (int)ViewType.Elevation, Name, true);
        }
    }
}