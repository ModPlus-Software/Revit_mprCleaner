namespace mprCleaner.WipeOptions
{
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.UI;

    public class WipeOptionFactory
    {
        private List<WipeOption> _wipeOptions;

        public List<WipeOption> GetWipeOptions(UIApplication uiApp)
        {
            if (_wipeOptions == null)
            {
                var uiDoc = uiApp.ActiveUIDocument;
                var doc = uiDoc.Document;
                _wipeOptions = new List<WipeOption>
                        {
                            // все неиспользуемые
                            new RemoveAllUnused(doc),

                            // все группы
                            new RemoveAllGroups(doc),

                            // все зависимости
                            new RemoveAllConstraints(uiApp),

                            // марки фасадов
                            new RemoveAllElevationMarkers(doc),

                            // связанные файлы
                            new RemoveAllExternalLinks(doc),
#if !R2015 && !R2016 && !R2017
                            // координационные модели
                            new RemoveCoordinationModels(doc),
#endif

                            // штриховки 
                            new RemoveAllModelPatterns(doc),

                            // штриховки
                            new RemoveAllDraftingPatterns(doc),

                            // все фильтры
                            new RemoveAllFilters(doc),

                            // все импортированные типы линий
                            new RemoveAllImportedLine(doc),

                            // материалы
                            new RemoveAllMaterials(doc),

                            // материалы (для визуализации)
                            new RemoveAllRenderMaterials(doc),

                            // опорные плоскости
                            new RemoveAllReferencePlanes(doc),

                            // разделители помещений
                            new RemoveAllRoomSeparationLines(doc),

                            // границы зон
                            new RemoveAllAreaSeparationLines(doc),

                            // области видимости
                            new RemoveAllScopeBoxes(doc),

                            // все помещения
                            new RemoveAllRooms(doc),

                            // все листы (кроме открытых)
                            new RemoveAllSheets(uiDoc),

                            // все шаблоны видов
                            new RemoveAllViewTemplates(doc),

                            // все виды (любого типа, кроме открытых видов и листов)
                            new RemoveAllViews(uiDoc),
                            new RemoveAllThreed(uiDoc),
                            new RemoveAllAreas(doc),
                            new RemoveAllPlans(uiDoc),
                            new RemoveAllRcps(uiDoc),
                            new RemoveAllEngplans(uiDoc),
                            new RemoveAllAreaPlans(uiDoc),
                            new RemoveAllDrafting(uiDoc),
                            new RemoveAllSections(uiDoc),
                            new RemoveAllDetailSection(uiDoc),
                            new RemoveAllElevations(uiDoc),
                            new RemoveAllSchedules(uiDoc),
                            new RemoveAllLegends(uiDoc),
                            new RemoveAllUnreferencedPlans(uiDoc),
                            new RemoveAllUnreferencedRcps(uiDoc),
                            new RemoveAllUnreferencedEngplans(uiDoc),
                            new RemoveAllUnreferencedAreaPlans(uiDoc),
                            new RemoveAllUnreferencedThreed(uiDoc),
                            new RemoveAllUnreferencedDrafting(uiDoc),
                            new RemoveAllUnreferencedSections(uiDoc),
                            new RemoveAllUnreferencedDetailSections(uiDoc),
                            new RemoveAllUnreferencedElevations(uiDoc),
                        };
                _wipeOptions = _wipeOptions.Concat(HelperMethods.GetWorksetCleaners(doc)).ToList();
            }

            return _wipeOptions;
        }
    }
}
