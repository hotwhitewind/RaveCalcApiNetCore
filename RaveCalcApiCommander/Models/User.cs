using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Models
{
    public class User
    {
        [Required(ErrorMessage = "Необходимо указать имя пользователя")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Необходимо указать пароль пользователя")]
        public string Password { get; set; }
    }
}
