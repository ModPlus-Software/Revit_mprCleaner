namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using ModPlusAPI;

    internal class RemoveAllExternalLinks : WipeOption
    {
        readonly Document _doc;

        internal RemoveAllExternalLinks(Document doc, string wipeArgs = null)
        {
            // Удалить все связанные файлы
            Name = Language.GetItem(RevitCommand.LangItem, "w11");
            WipeArgs = wipeArgs;
            _doc = doc;
        }

        internal override int Execute(string args = null)
        {
            string filepath = _doc.PathName;
            if (filepath != null)
            {
                IList<Element> xRefLinks = new List<Element>();
                ModelPath modelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(filepath);
                try
                {
                    TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
                    ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();
                    foreach (ElementId xRefId in externalReferences)
                    {
                        Element externalReference = _doc.GetElement(xRefId);
                        if (ConfirmRemoval(externalReference))
                            xRefLinks.Add(externalReference);
                    }
                    return HelperMethods.RemoveElements(Name, _doc, xRefLinks);
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                TaskDialog.Show(Name, "Модель должна быть сохранена для удаления внешних ссылок.");
            }
            return 0;
        }

        private bool ConfirmRemoval(Element linkEl)
        {
            return linkEl is RevitLinkType || linkEl is CADLinkType;
        }

    }
}
