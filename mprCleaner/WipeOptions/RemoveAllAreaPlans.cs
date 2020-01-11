namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllAreaPlans : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllAreaPlans(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только планы зонирования)
            Name = Language.GetItem(RevitCommand.LangItem, "w1");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.AreaPlan, Name);
        }
    }
}