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
    }
}
