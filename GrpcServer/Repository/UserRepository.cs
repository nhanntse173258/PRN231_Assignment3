using GrpcServer.Models;

namespace GrpcServer.Repository
{
    public class UserRepository
    {
        private readonly List<User> _users = new List<User>();
        public UserRepository()
        {
            // Adding some test users for testing purposes
            _users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "admin",
                Password = "123",
                Role = "Admin"
            });
            _users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "user",
                Password = "123", 
                Role = "User"
            });
            _users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "user1",
                Password = "123",
                Role = "User"
            });
        }

        public User CreateUser(User user)
        {
            user.Id = Guid.NewGuid().ToString(); // Assign a new ID
            _users.Add(user);
            return user;
        }

        public User GetUserById(string id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public User UpdateUser(User user)
        {
            var existingUser = GetUserById(user.Id);
            if (existingUser != null)
            {
                existingUser.Username = user.Username; // Update fields as necessary
                existingUser.Password = user.Password;
                existingUser.Role = user.Role; // Update Role
            }
            return existingUser;
        }

        public User DeleteUser(string id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                _users.Remove(user);
            }
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _users;
        }
    }
}
