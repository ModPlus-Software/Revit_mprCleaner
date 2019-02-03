namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Reflection;
    using Autodesk.Revit.DB;
    using ModPlusAPI;

    internal class RemoveAllUnused : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllUnused(Document doc, string wipeArgs = null)
        {
            // Удалить все неиспользуемые элементы
            Name = Language.GetItem(RevitCommand.LangItem, "w38");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            MethodInfo getUnusedAppearances = _doc.GetType().GetMethod("GetUnusedAppearances", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getUnusedMaterials = _doc.GetType().GetMethod("GetUnusedMaterials", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getUnusedFamilies = _doc.GetType().GetMethod("GetUnusedFamilies", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getUnusedImportCategories = _doc.GetType().GetMethod("GetUnusedImportCategories", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getUnusedStructures = _doc.GetType().GetMethod("GetUnusedStructures", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getUnusedSymbols = _doc.GetType().GetMethod("GetUnusedSymbols", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getUnusedThermals = _doc.GetType().GetMethod("GetUnusedThermals", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo getNonDeletableUnusedElements = _doc.GetType().GetMethod("GetNonDeletableUnusedElements", BindingFlags.NonPublic | BindingFlags.Instance);

            int num = 0;
            while (true)
            {
                HashSet<ElementId> hashSet = new HashSet<ElementId>();

                var unusedAppearances = getUnusedAppearances?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current in unusedAppearances)
                {
                    hashSet.Add(current);
                }

                var unusedMaterials = getUnusedMaterials?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current2 in unusedMaterials)
                {
                    hashSet.Add(current2);
                }

                var unusedFamilies = getUnusedFamilies?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current3 in unusedFamilies)
                {
                    hashSet.Add(current3);
                }

                var unusedImportCategories = getUnusedImportCategories?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current4 in unusedImportCategories)
                {
                    hashSet.Add(current4);
                }

                var unusedStructures = getUnusedStructures?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current5 in unusedStructures)
                {
                    hashSet.Add(current5);
                }

                var unusedSymbols = getUnusedSymbols?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current6 in unusedSymbols)
                {
                    hashSet.Add(current6);
                }

                var unusedThermals = getUnusedThermals?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current7 in unusedThermals)
                {
                    hashSet.Add(current7);
                }

                var nonDeletableUnusedElements = getNonDeletableUnusedElements?.Invoke(_doc, null) as ICollection<ElementId>;
                foreach (ElementId current8 in nonDeletableUnusedElements)
                {
                    hashSet.Remove(current8);
                }
                
                if (hashSet.Count != num && hashSet.Count != 0)
                {
                    num = hashSet.Count;
                    using (Transaction transaction = new Transaction(_doc, "purge unused"))
                    {
                        transaction.Start();
                        _doc.Delete(hashSet);
                        transaction.Commit();
                        continue;
                    }
                }
                break;
            }

            return num;
        }
    }
}
