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

        public async Task<bool> SaveRefreshToken(User user, RefreshToken refresh)
        {
            try
            {
                var userFind = await _mongoDbRepository.FindOneAsync(c => c.UserName == user.UserName && c.Password == user.Password);
                if (userFind != null)
                {
                    if(userFind.RefreshTokens == null)
                    {
                        userFind.RefreshTokens = new List<RefreshToken>();
                    }
                    userFind.RefreshTokens.Add(refresh);
                    await _mongoDbRepository.ReplaceOneAsync(userFind);
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "SaveRefreshToken error");
                return false;
            }
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refresh)
        {
            try
            {
                var userFind = await _mongoDbRepository.FindOneAsync(c => c.RefreshTokens.Any(x => x.Token == refresh.Token));
                if (userFind != null)
                {
                    userFind.RefreshTokens.RemoveAll(c => c.Token == refresh.Token);
                    await _mongoDbRepository.ReplaceOneAsync(userFind);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveRefreshToken error");
                return false;
            }
        }

        public async Task<User> CheckRefreshToken(RefreshToken refresh)
        {
            try
            {
                var userFind = await _mongoDbRepository.FindOneAsync(c => c.RefreshTokens.Any(x => x.Token == refresh.Token));
                if (userFind != null)
                {
                    var findRefresh = userFind.RefreshTokens.Find(c => c.Token == refresh.Token);
                    if (findRefresh != null && findRefresh.IsActive)
                    {
                        if(findRefresh.IsActive)
                            return userFind;
                        else
                        {
                            //remove old refresh token
                            userFind.RefreshTokens.Remove(refresh);
                            await _mongoDbRepository.ReplaceOneAsync(userFind);
                        }
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveRefreshToken error");
                return null;
            }
        }
    }
}
