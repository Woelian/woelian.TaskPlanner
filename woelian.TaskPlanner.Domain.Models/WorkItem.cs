using woelian.TaskPlanner.Domain.Models.Enums;

namespace woelian.TaskPlanner.Domain.Models
{
    public class WorkItem : IComparable<WorkItem>
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public Complexity Complexity { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        public WorkItem Clone() =>
            new WorkItem
            {
                Id = Id,
                CreationDate = CreationDate,
                DueDate = DueDate,
                Priority = Priority,
                Complexity = Complexity,
                Title = Title,
                Description = Description,
                IsCompleted = IsCompleted
            };

        public int CompareTo(WorkItem other)
        {
            if (Priority > other.Priority)
                return -1;
            else if (Priority < other.Priority)
                return 1;

            if (DueDate < other.DueDate)
                return -1;
            else if (DueDate > other.DueDate)
                return 1;

            return string.Compare(Title, other.Title, StringComparison.CurrentCultureIgnoreCase);
        }

        public override string ToString() =>
            $"{Title}, due {DueDate:dd.MM.yyyy}, {Priority.ToString().ToLowerInvariant()} priority";
    }
}