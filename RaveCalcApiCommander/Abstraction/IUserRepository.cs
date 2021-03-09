using MongoDB.Bson;
using RaveCalcApiCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Abstraction
{
    public interface IUserRepository
    {
        public Task<User> GetUser(ObjectId Id);
        public Task<User> IsAuthentificate(string Login, string Password);
        public Task<bool> SaveRefreshToken(User user, RefreshToken refresh);
        public Task<bool> RemoveRefreshToken(RefreshToken refresh);
        public Task<User> CheckRefreshToken(RefreshToken refresh);
    }
}
