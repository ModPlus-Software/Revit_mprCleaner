namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllEngplans : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllEngplans(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только планы несущих конструкций)
            Name = Language.GetItem(RevitCommand.LangItem, "w10");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewPlan), (int)ViewType.EngineeringPlan, Name);
        }
    }
}