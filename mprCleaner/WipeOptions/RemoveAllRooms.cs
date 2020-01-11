namespace mprCleaner.WipeOptions
{
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllRooms : WipeOption
    {
        private readonly Document _doc;

        internal RemoveAllRooms(Document doc, string wipeArgs = null)
        {
            // Удалить все помещения
            Name = Language.GetItem(RevitCommand.LangItem, "w22");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        /// <inheritdoc/>
        internal override int Execute(string args = null)
        {
            var rooms = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .ToElements();
            if (rooms.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, rooms);
            else
                return 0;
        }
    }
}
