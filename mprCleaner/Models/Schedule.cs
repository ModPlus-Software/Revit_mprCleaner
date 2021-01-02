namespace mprCleaner.Models
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    public class Schedule : BaseSelectable
    {
        public Schedule(ViewSchedule viewSchedule)
        {
            Name = viewSchedule.Name;
            Id = viewSchedule.Id;
            PlacedOnSheets = new List<string>();
        }
        
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Id
        /// </summary>
        public ElementId Id { get; }
        
        /// <summary>
        /// Листы, на которых размещена спецификация
        /// </summary>
        public List<string> PlacedOnSheets { get; }
    }
}
