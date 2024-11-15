using movieReservationSystem.Models;
using movieReservationSystem.Repositories;

namespace movieReservationSystem.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Authenticate(string username, string password)
        {
            return _userRepository.GetUserByUsernameAndPassword(username, password);
        }

        public User Register(string username, string password)
        {
            if (_userRepository.UsernameExists(username))
            {
                throw new System.Exception("Username already exists.");
            }

            var user = new User { Username = username, Password = password };
            return _userRepository.AddUser(user);
        }
    }
}