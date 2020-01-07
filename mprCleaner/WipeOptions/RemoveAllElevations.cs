namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllElevations : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllElevations(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только фасады)
            Name = Language.GetItem(RevitCommand.LangItem, "w9");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewSection), (int)ViewType.Elevation, Name);
        }
    }
}