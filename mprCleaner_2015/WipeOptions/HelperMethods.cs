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
                        //
                    }
                }
                return false;
            }

            using (var tr = new Transaction(uiDoc, actionTitle))
            {
                if (TransactionStatus.Started == tr.Start())
                {
                    foreach (Element elementToRemove in elementsToRemove)
                    {
                        if (RemoveElement(elementToRemove))
                            count++;
                    }
                    if (TransactionStatus.Committed == tr.Commit())
                        return count;
                    else
                        tr.RollBack();
                }
            }
            return 0;
        }

        internal static List<WipeOption> GetWorksetCleaners(Document doc)
        {
            List<WipeOption> worksetFuncs = new List<WipeOption>();

            //if model is workshared, get a list of current worksets
            if (doc.IsWorkshared)
            {
                FilteredWorksetCollector cl = new FilteredWorksetCollector(doc);
                FilteredWorksetCollector worksetList = cl.OfKind(WorksetKind.UserWorkset);

                //duplicate the workset element remover function for each workset
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
            IList<UIView> openUIViews = uiDoc.GetOpenUIViews();
            List<ElementId> openViews = new List<ElementId>();
            foreach (UIView ov in openUIViews)
                openViews.Add(ov.ViewId);
            List<ViewType> readonlyViews = new List<ViewType>()
            {
                  ViewType.ProjectBrowser,
                  ViewType.SystemBrowser,
                  ViewType.Undefined,
                  ViewType.DrawingSheet,
                  ViewType.Internal
            };
            bool IsReferenced(View view)
            {
                IList<Element> viewRefs = new FilteredElementCollector(uiDoc.Document)
                           .OfCategory(BuiltInCategory.OST_ReferenceViewer)
                           .WhereElementIsNotElementType()
                           .ToElements();
                List<ElementId> viewRefsIds = new List<ElementId>();
                foreach (Element viewRef in viewRefs)
                {
                    Parameter refParam = viewRef.get_Parameter(BuiltInParameter.REFERENCE_VIEWER_TARGET_VIEW);
                    viewRefsIds.Add(refParam.AsElementId());
                }
                Parameter refSheet = view.get_Parameter(BuiltInParameter.VIEW_REFERENCING_SHEET);
                Parameter refViewport = view.get_Parameter(BuiltInParameter.VIEW_REFERENCING_DETAIL);
                if (!(refSheet is null)
                        && !(refViewport is null)
                        && refSheet.AsString() != ""
                        && refViewport.AsString() != ""
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
                    if (ViewType.ThreeD == view.ViewType && "{3D}" == view.Name)
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
