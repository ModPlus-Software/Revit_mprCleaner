namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedDrafting : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedDrafting(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только чертежные виды)
            Name = Language.GetItem(RevitCommand.LangItem, "w31");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewDrafting), 0, Name, true);
        }
    }
}