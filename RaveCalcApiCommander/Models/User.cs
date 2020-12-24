using RaveCalcApiCommander.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaveCalcApiCommander.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Roles { admin, user};

    [BsonCollection("users")]
    public class User : Document
    {
        [Required(ErrorMessage = "Необходимо указать имя пользователя")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Необходимо указать пароль пользователя")]
        public string Password { get; set; }
        public Roles Role { get; set; }
    }
}
