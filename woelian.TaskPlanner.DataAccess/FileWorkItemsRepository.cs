using Newtonsoft.Json;
using woelian.TaskPlanner.Domain.Models;
using woelian.TaskPlanner.DataAccess.Abstractions;

namespace woelian.TaskPlanner.DataAccess
{
    public class FileWorkItemsRepository : IWorkItemsRepository
    {
        private const string _fileName = "work-items.json";
        private const string _directotyPath = "Resources";

        private static string _path => $@"{_directotyPath}\{_fileName}";

        private readonly Dictionary<Guid, WorkItem> _workItems = new Dictionary<Guid, WorkItem>();

        public FileWorkItemsRepository()
        {
            if (!File.Exists(_path))
            {
                Directory.CreateDirectory(_directotyPath);
                File.Create(_path).Close();

                return;
            }

            string json = File.ReadAllText(_path);
            WorkItem[] workItems = JsonConvert.DeserializeObject<WorkItem[]>(json);

            if (workItems is not null)
                for (int i = 0; i < workItems.Length; i++)
                    _workItems.Add(workItems[i].Id, workItems[i]);
        }

        public Guid Add(WorkItem workItem)
        {
            WorkItem clonedItem = workItem.Clone();
            Guid id = Guid.NewGuid();
            clonedItem.Id = id;
            _workItems.Add(id, clonedItem);

            return id;
        }

        public WorkItem Get(Guid id) => _workItems[id].Clone();

        public WorkItem[] GetAll() => _workItems.Select(itemRecord => itemRecord.Value.Clone()).ToArray();

        public bool Remove(Guid id)
        {
            if (!_workItems.ContainsKey(id))
                return false;

            _workItems.Remove(id);

            return true;
        }

        public void SaveChanges()
        {
            WorkItem[] workItems = GetAll();
            string json = JsonConvert.SerializeObject(workItems);
            File.WriteAllText(_path, json);
        }

        public bool Update(WorkItem workItem)
        {
            if (!_workItems.ContainsKey(workItem.Id))
                return false;

            _workItems[workItem.Id] = workItem.Clone();

            return true;
        }
    }
}
