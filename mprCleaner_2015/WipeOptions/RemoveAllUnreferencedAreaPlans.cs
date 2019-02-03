namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedAreaPlans : WipeOption
    {
        readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedAreaPlans(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только планы зонирования)
            Name = Language.GetItem(RevitCommand.LangItem, "w29");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.AreaPlan, Name, true);
        }
    }
}