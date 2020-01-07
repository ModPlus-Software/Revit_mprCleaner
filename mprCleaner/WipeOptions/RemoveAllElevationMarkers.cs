namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllElevationMarkers : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllElevationMarkers(Document doc, string wipeArgs = null)
        {
            // Удалить все марки фасадов
            Name = Language.GetItem(RevitCommand.LangItem, "w8");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            IList<Element> elevMarkers = new FilteredElementCollector(_doc)
                .OfClass(typeof(ElevationMarker))
                .WhereElementIsNotElementType()
                .Where(elevMarker => ConfirmRemoval(elevMarker as ElevationMarker))
                .ToList();
            if (elevMarkers.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, elevMarkers);
            return 0;

            bool ConfirmRemoval(ElevationMarker elevMarker)
            {
                return elevMarker.CurrentViewCount == 0;
            }
        }
    }
}