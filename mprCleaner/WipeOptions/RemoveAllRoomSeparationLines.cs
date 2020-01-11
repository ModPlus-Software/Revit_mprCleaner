namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllRoomSeparationLines : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllRoomSeparationLines(Document doc, string wipeArgs = null)
        {
            // Удалить все линии разделителей помещений
            Name = Language.GetItem(RevitCommand.LangItem, "w23");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var roomLines = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_RoomSeparationLines)
                .WhereElementIsNotElementType()
                .ToElements();
            if (roomLines.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, roomLines);
            else
                return 0;
        }
    }
}
