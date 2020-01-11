namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllUnreferencedThreed : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllUnreferencedThreed(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды без ссылок (только 3D виды)
            Name = Language.GetItem(RevitCommand.LangItem, "w37");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(View3D), (int)ViewType.ThreeD, Name, true);
        }
    }
}