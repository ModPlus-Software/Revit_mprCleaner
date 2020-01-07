namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllDrafting : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllDrafting(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только чертежные виды)
            Name = Language.GetItem(RevitCommand.LangItem, "w6");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewDrafting), 0, Name);
        }
    }
}