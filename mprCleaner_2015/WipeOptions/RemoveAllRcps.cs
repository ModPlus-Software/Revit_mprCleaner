namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllRcps : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllRcps(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только планы потолков)
            Name = Language.GetItem(RevitCommand.LangItem, "w19");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.CeilingPlan, Name);
        }
    }
}