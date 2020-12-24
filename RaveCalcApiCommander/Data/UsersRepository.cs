using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using RaveCalcApiCommander.Abstraction;
using RaveCalcApiCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Data
{
    public class UsersRepository : IUserRepository
    {
        private readonly IMongoDbRepository<User> _mongoDbRepository;
        private readonly ILogger<UsersRepository> _logger;

        public UsersRepository(ILogger<UsersRepository> logger, IMongoDbRepository<User> mongoDbRepository)
        {
            _logger = logger;
            _mongoDbRepository = mongoDbRepository;
        }

        public async Task<User> GetUser(ObjectId Id)
        {
            try
            {
                return await _mongoDbRepository.FindOneAsync(c => c.Id == Id);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "GetUser error");
                return null;
            }
        }

        public async Task<User> IsAuthentificate(string Login, string Password)
        {
            try
            {
                var passHash = ComputeHash(Password);
                var user = await _mongoDbRepository.FindOneAsync(c => c.UserName == Login && c.Password == passHash);
                return user;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "User IsAuthentificate error");
                return null;
            }
        }

        public static string ComputeHash(string password)
        {
            SHA384CryptoServiceProvider hashAlgorithm = new SHA384CryptoServiceProvider();
            return Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
