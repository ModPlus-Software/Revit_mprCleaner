namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedDetailSections : WipeOption
    {
        readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedDetailSections(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только узлы)
            Name = Language.GetItem(RevitCommand.LangItem, "w30");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewSection), (int)ViewType.Detail, Name, true);
        }
    }
}