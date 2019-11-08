namespace mprCleaner.WipeOptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;

    internal class HelperMethods
    {
        internal static int RemoveElements(string actionTitle, Document uiDoc, IList<Element> elementsToRemove)
        {
            var count = 0;

            bool RemoveElement(Element remEl)
            {
                if (!(remEl is null))
                {
                    try
                    {
                        uiDoc.Delete(remEl.Id);
                        return true;
                    }
                    catch
                    {
                        // ignore
                    }
                }

                return false;
            }

            using (var tr = new Transaction(uiDoc, actionTitle))
            {
                if (tr.Start() == TransactionStatus.Started)
                {
                    foreach (var elementToRemove in elementsToRemove)
                    {
                        if (RemoveElement(elementToRemove))
                            count++;
                    }

                    if (tr.Commit() == TransactionStatus.Committed)
                        return count;
                    else
                        tr.RollBack();
                }
            }

            return 0;
        }

        internal static List<WipeOption> GetWorksetCleaners(Document doc)
        {
            var worksetFuncs = new List<WipeOption>();

            // if model is workshared, get a list of current worksets
            if (doc.IsWorkshared)
            {
                var cl = new FilteredWorksetCollector(doc);
                var worksetList = cl.OfKind(WorksetKind.UserWorkset);

                // duplicate the workset element remover function for each workset
                foreach (var workset in worksetList)
                {
                    WipeOption wipeOption = new WorksetRemover(doc, workset.Name);
                    worksetFuncs.Add(wipeOption);
                }
            }

            return worksetFuncs;
        }

        internal static int PurgeAllViews(UIDocument uiDoc, Type viewClassToPurge, int viewTypeToPurge, string actionTitle, bool keepReferenced = false)
        {
            var openUiViews = uiDoc.GetOpenUIViews();
            var openViews = new List<ElementId>();
            foreach (var ov in openUiViews)
                openViews.Add(ov.ViewId);
            var readonlyViews = new List<ViewType>()
            {
                  ViewType.ProjectBrowser,
                  ViewType.SystemBrowser,
                  ViewType.Undefined,
                  ViewType.DrawingSheet,
                  ViewType.Internal
            };
            bool IsReferenced(View view)
            {
                var viewRefs = new FilteredElementCollector(uiDoc.Document)
                           .OfCategory(BuiltInCategory.OST_ReferenceViewer)
                           .WhereElementIsNotElementType()
                           .ToElements();
                var viewRefsIds = new List<ElementId>();
                foreach (var viewRef in viewRefs)
                {
                    var refParam = viewRef.get_Parameter(BuiltInParameter.REFERENCE_VIEWER_TARGET_VIEW);
                    viewRefsIds.Add(refParam.AsElementId());
                }

                var refSheet = view.get_Parameter(BuiltInParameter.VIEW_REFERENCING_SHEET);
                var refViewport = view.get_Parameter(BuiltInParameter.VIEW_REFERENCING_DETAIL);
                if ((!(refSheet is null)
                        && !(refViewport is null)
                        && refSheet.AsString() != string.Empty
                        && refViewport.AsString() != string.Empty)
                        || viewRefsIds.Contains(view.Id))
                    return true;
                return false;
            }

            bool ConfirmRemoval(View view)
            {
                if (view.GetType().IsSubclassOf(viewClassToPurge) ||
                    view.GetType() == viewClassToPurge)
                {
                    if (viewTypeToPurge != 0 && (int)view.ViewType != viewTypeToPurge)
                        return false;
                    if (readonlyViews.Contains(view.ViewType))
                        return false;
                    if (view.IsTemplate)
                        return false;
                    if (view.ViewType == ViewType.ThreeD && view.Name == "{3D}")
                        return false;
                    if (view.Name.Contains("<"))
                        return false;
                    if (openViews.Contains(view.Id))
                        return false;
                    if (keepReferenced && IsReferenced(view))
                        return false;
                    return true;
                }

                return false;
            }

            IList<Element> views = new FilteredElementCollector(uiDoc.Document)
               .OfClass(viewClassToPurge)
               .WhereElementIsNotElementType()
               .Where(v => ConfirmRemoval(v as View))
               .ToList();

            return RemoveElements(actionTitle, uiDoc.Document, views);
        }
    }
}
