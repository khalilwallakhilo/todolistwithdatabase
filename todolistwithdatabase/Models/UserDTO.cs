﻿using System.ComponentModel.DataAnnotations;

namespace todolistwithdatabase.Models
{
    public class UserDTO
    {
        
        public string Username { get; set; }
        
        public string Password { get; set; }
       
        public string Role { get; set; }
    }
}
