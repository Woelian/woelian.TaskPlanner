using woelian.TaskPlanner.DataAccess;
using woelian.TaskPlanner.Domain.Logic;
using woelian.TaskPlanner.Domain.Models;
using woelian.TaskPlanner.Domain.Models.Enums;

namespace woelian.TaskPlanner.ConsoleRunner
{
    internal static class Program
    {
        private static FileWorkItemsRepository _workItemsRepository;
        private static SimpleTaskPlanner _simpleTaskPlanner;
        private static bool _isAppRunning = true;

        public static void Main(string[] args)
        {
            Initialize();
            string? input;

            while (_isAppRunning)
            {
                Console.WriteLine("[A]dd work item");
                Console.WriteLine("[B]uild a plan");
                Console.WriteLine("[M]ark work item as completed");
                Console.WriteLine("[R]emove a work item");
                Console.WriteLine("[Q]uit the app");

                input = Console.ReadLine()?.ToLower();

                switch (input)
                {
                    case "a":
                        AddWorkItem();
                        break;
                    case "b":
                        BuildPlan();
                        break;
                    case "m":
                        MarkWorkItemCompletion(true);
                        break;
                    case "r":
                        RemoveWorkItem();
                        break;
                    case "q":
                        Quit();
                        break;
                    default:
                        Console.WriteLine($"No action for: {input}");
                        break;
                }
            }
        }

        private static void Initialize()
        {
            _workItemsRepository = new FileWorkItemsRepository();
            _simpleTaskPlanner = new SimpleTaskPlanner(_workItemsRepository);
        }

        private static void AddWorkItem() => _workItemsRepository.Add(GetWorkItem());

        private static void BuildPlan() => PrintWorkItems(_simpleTaskPlanner.CreatePlan());

        private static void MarkWorkItemCompletion(bool isCompleted) =>
            ProcessWorkItem("Enter index of item to mark it completion:",
                item =>
                {
                    item.IsCompleted = isCompleted;
                    _workItemsRepository.Update(item);
                });

        private static void RemoveWorkItem() =>
            ProcessWorkItem("Enter index of item to remove:",
                item => _workItemsRepository.Remove(item.Id));

        private static void ProcessWorkItem(string processMessage, Action<WorkItem> processAction)
        {
            WorkItem[] workItems = _workItemsRepository.GetAll();
            PrintWorkItems(workItems);
            Console.WriteLine(processMessage);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int itemIndex))
                processAction(workItems[itemIndex]);
        }

        private static void Quit()
        {
            _workItemsRepository.SaveChanges();
            _isAppRunning = false;
        }

        private static void PrintWorkItems(WorkItem[] workItems)
        {
            Console.WriteLine("\n");
            int i = 0;
            workItems
                .ToList()
                .ForEach(item =>
                {
                    Console.WriteLine($"[{i}] {item}");
                    ++i;
                });
            Console.WriteLine("\n");
        }

        private static WorkItem GetWorkItem()
        {
            Console.WriteLine($"\nEnter data for item");

            var workItem = new WorkItem
            {
                CreationDate = DateTime.Now,
                DueDate = GetValue("Enter due date:", (string s) => DateTime.Parse(s)),
                Title = GetValue("Enter title:", (string s) => s),
                Description = GetValue("Enter description:", (string s) => s),
                Complexity = GetValue("Enter complexity:", (string s) => Enum.Parse<Complexity>(s, true)),
                Priority = GetValue("Enter priority:", (string s) => Enum.Parse<Priority>(s, true))
            };

            return workItem;
        }

        private static T GetValue<T>(string message, Func<string, T> getValueAction)
        {
            Console.WriteLine(message);
            string? input = Console.ReadLine();

            return getValueAction(input);
        }
    }
}