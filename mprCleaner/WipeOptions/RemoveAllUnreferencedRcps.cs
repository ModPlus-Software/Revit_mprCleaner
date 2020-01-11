namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedRcps : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedRcps(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только планы потолков)
            Name = Language.GetItem(RevitCommand.LangItem, "w35");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.CeilingPlan, Name, true);
        }
    }
}