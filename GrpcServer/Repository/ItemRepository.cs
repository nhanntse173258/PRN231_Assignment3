using GrpcServer.Models;

namespace GrpcServer.Repository
{
    public class ItemRepository
    {
        private readonly List<Item> _items = new List<Item>();
        public ItemRepository()
        {
            // Seed with some items related to coding
            SeedItems();
        }

        private void SeedItems()
        {
            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Clean Code",
                Description = "A handbook of agile software craftsmanship by Robert C. Martin"
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "The Pragmatic Programmer",
                Description = "A journey through the career of a software developer, covering topics like debugging and career growth."
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Visual Studio Code",
                Description = "A powerful, lightweight code editor used by developers for web and cloud-based applications."
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Git",
                Description = "A version control system for tracking changes in computer files and coordinating work on those files among multiple people."
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Docker",
                Description = "An open platform for developing, shipping, and running applications in containers."
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Node.js",
                Description = "A JavaScript runtime built on Chrome's V8 JavaScript engine, used for building scalable network applications."
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "JavaScript: The Good Parts",
                Description = "A concise guide to the most elegant features of JavaScript, focusing on the best aspects of the language."
            });

            _items.Add(new Item
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Kubernetes",
                Description = "An open-source system for automating deployment, scaling, and managing containerized applications."
            });
        }

        public Item GetItem(string id)
        {
            return _items.FirstOrDefault(i => i.Id.Equals(id));
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _items;
        }

        public void AddItem(Item item)
        {
            item.Id = Guid.NewGuid().ToString(); // Generate a new unique string ID
            _items.Add(item);
        }

        public void UpdateItem(Item item)
        {
            var existingItem = GetItem(item.Id);
            if (existingItem != null)
            {
                existingItem.Name = item.Name;
                existingItem.Description = item.Description;
            }
        }

        public void DeleteItem(string id)
        {
            var item = GetItem(id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }
    }
}
