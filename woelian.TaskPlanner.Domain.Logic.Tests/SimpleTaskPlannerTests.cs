using Moq;
using Xunit;
using woelian.TaskPlanner.DataAccess.Abstractions;
using woelian.TaskPlanner.Domain.Models;
using woelian.TaskPlanner.Domain.Models.Enums;

namespace woelian.TaskPlanner.Domain.Logic.Tests
{
    public class SimpleTaskPlannerTests
    {
        private readonly SimpleTaskPlanner _systemUnderTest;
        private readonly Mock<IWorkItemsRepository> _workItemsRepoMock;
        private readonly WorkItem[] _workItems;

        public SimpleTaskPlannerTests()
        {
            _workItemsRepoMock = new Mock<IWorkItemsRepository>();
            _systemUnderTest = new SimpleTaskPlanner(_workItemsRepoMock.Object);

            _workItems = new WorkItem[]
            {
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    CreationDate = new DateTime(2022, 11, 1),
                    DueDate = new DateTime(2022, 11, 9),
                    Priority = Priority.High,
                    Complexity = Complexity.Hours,
                    Title = "Buy milk",
                    Description = "-",
                    IsCompleted = true
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    CreationDate = new DateTime(2022, 11, 1),
                    DueDate = new DateTime(2022, 11, 9),
                    Priority = Priority.Medium,
                    Complexity = Complexity.Minutes,
                    Title = "Buy fruits",
                    Description = "-",
                    IsCompleted = false
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    CreationDate = new DateTime(2022, 11, 1),
                    DueDate = new DateTime(2022, 11, 9),
                    Priority = Priority.Medium,
                    Complexity = Complexity.Minutes,
                    Title = "Buy bread",
                    Description = "-",
                    IsCompleted = false
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    CreationDate = new DateTime(2022, 11, 1),
                    DueDate = new DateTime(2022, 11, 14),
                    Priority = Priority.Urgent,
                    Complexity = Complexity.None,
                    Title = "Fuck some chiks",
                    Description = "-",
                    IsCompleted = false
                },
                new WorkItem
                {
                    Id = Guid.NewGuid(),
                    CreationDate = new DateTime(2022, 11, 1),
                    DueDate = new DateTime(2022, 11, 15),
                    Priority = Priority.Urgent,
                    Complexity = Complexity.Minutes,
                    Title = "Feed dog",
                    Description = "-",
                    IsCompleted = false
                }
            };
        }

        [Fact]
        public void CreatePlan_ShouldReturnSortedItems()
        {
            _workItemsRepoMock.Setup(repo => repo.GetAll()).Returns(_workItems);

            WorkItem[] planedItems = _systemUnderTest.CreatePlan();

            for (int i = 0; i < planedItems.Length - 1; i++)
                Assert.True(CompareWorkItems(planedItems[i], planedItems[i + 1]) == -1);

            static int CompareWorkItems(WorkItem first, WorkItem second)
            {
                if (first.Priority > second.Priority)
                    return -1;
                else if (first.Priority < second.Priority)
                    return 1;

                if (first.DueDate < second.DueDate)
                    return -1;
                else if (first.DueDate > second.DueDate)
                    return 1;

                return string.Compare(first.Title, second.Title, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        [Fact]
        public void CreatePlan_ShouldIncludeAllUncompletedItems()
        {
            _workItemsRepoMock.Setup(repo => repo.GetAll()).Returns(_workItems);

            WorkItem[] planedItems = _systemUnderTest.CreatePlan();

            IEnumerable<WorkItem> uncompletedItems =
                _workItemsRepoMock.Object.GetAll()
                .Where(item => !item.IsCompleted);

            Assert.All(planedItems, item => Assert.Contains(item, uncompletedItems));
        }

        [Fact]
        public void CreatePlan_ShouldNotIncludeCompletedItems()
        {
            _workItemsRepoMock.Setup(repo => repo.GetAll()).Returns(_workItems);

            WorkItem[] planedItems = _systemUnderTest.CreatePlan();

            Assert.All(planedItems, item => Assert.False(item.IsCompleted));
        }
    }
}
