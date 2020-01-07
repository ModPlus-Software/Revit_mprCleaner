namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllThreed : WipeOption
    {
        private readonly UIDocument _uiDoc;

        internal RemoveAllThreed(UIDocument uiDoc, string wipeArgs = null)
        {
            // Удалить все виды (только 3D виды)
            Name = Language.GetItem(RevitCommand.LangItem, "w28");
            WipeArgs = wipeArgs;
            _uiDoc = uiDoc;
        }

        internal override int Execute(string args = null)
        {
            return HelperMethods.PurgeAllViews(_uiDoc, typeof(View3D), (int)ViewType.ThreeD, Name);
        }
    }
}