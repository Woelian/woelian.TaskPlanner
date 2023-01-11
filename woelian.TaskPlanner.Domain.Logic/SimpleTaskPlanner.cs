using woelian.TaskPlanner.DataAccess.Abstractions;
using woelian.TaskPlanner.Domain.Models;

namespace woelian.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemsRepository _workItemsRepository;

        public SimpleTaskPlanner(IWorkItemsRepository workItemsRepository)
        {
            _workItemsRepository = workItemsRepository;
        }

        public WorkItem[] CreatePlan()
        {
            List<WorkItem> workItemsList = 
                _workItemsRepository
                .GetAll()
                .Where(item => !item.IsCompleted)
                .ToList();
            workItemsList.Sort();

            return workItemsList.ToArray();
        }
    }
}
