namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllDetailSection : WipeOption
    {
        readonly UIDocument _uiDoc;

        internal RemoveAllDetailSection(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только узлы)
            Name = Language.GetItem(RevitCommand.LangItem, "w5");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewSection), (int)ViewType.Detail, Name);
        }
    }
}