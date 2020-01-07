namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedSections : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedSections(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только разрезы)
            Name = Language.GetItem(RevitCommand.LangItem, "w36");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewSection), (int)ViewType.Section, Name, true);
        }
    }
}