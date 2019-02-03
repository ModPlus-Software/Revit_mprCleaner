namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedEngplans : WipeOption
    {
        readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedEngplans(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только планы несущих конструкций)
            Name = Language.GetItem(RevitCommand.LangItem, "w33");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.EngineeringPlan, Name, true);
        }
    }
}