namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedPlans : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedPlans(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только планы этажей)
            Name = Language.GetItem(RevitCommand.LangItem, "w34");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.FloorPlan, Name, true);
        }
    }
}