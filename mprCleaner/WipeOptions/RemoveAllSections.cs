namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllSections : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllSections(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только разрезы)
            Name = Language.GetItem(RevitCommand.LangItem, "w26");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(ViewSection), (int)ViewType.Section, Name);
        }
    }
}