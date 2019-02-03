namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllGroups : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllGroups(Document doc, string wipeArgs = null)
        {
            // Удалить (и разгруппировать) все группы
            Name = Language.GetItem(RevitCommand.LangItem, "w13");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args)
        {
            IList<Element> groupTypes = new FilteredElementCollector(_doc)
                .OfClass(typeof(GroupType))
                .Where(gType => ConfirmRemoval(gType) == true)
                .ToList();
            IList<Element> groups = new FilteredElementCollector(_doc)
                .OfClass(typeof(Group))
                .ToElements();
            //ungroup all groups
            if (groups.Count > 0)
                using (var tr = new Transaction(_doc, "Разгруппировать все группы"))
                {
                    if (TransactionStatus.Started == tr.Start())
                    {
                        foreach (var element in groups)
                        {
                            var grp = (Group) element;
                            grp.UngroupMembers();
                        }

                        if (TransactionStatus.Committed != tr.Commit())
                            tr.RollBack();
                    }
                }
            //delete group types
            if (groupTypes.Count > 0)
                return HelperMethods.RemoveElements(Name, _doc, groupTypes);
            return 0;
        }

        private bool ConfirmRemoval(Element groupType)
        {
            return groupType != null & groupType?.Category.Id.IntegerValue == (int)BuiltInCategory.OST_IOSModelGroups;
        }

    }
}
