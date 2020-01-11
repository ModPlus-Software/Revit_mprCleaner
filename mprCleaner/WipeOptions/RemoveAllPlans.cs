namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllPlans : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllPlans(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только планы этажей)
            Name = Language.GetItem(RevitCommand.LangItem, "w18");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.FloorPlan, Name);
        }
    }
}