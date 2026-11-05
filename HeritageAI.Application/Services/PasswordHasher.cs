using BCrypt.Net;
using HeritageAI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Application.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        //public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
        public string Hash(string password) { 
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool Verify(string password, string hashedPassword) { 
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
      //  public bool Verify(string password, string hashedPassword) =>
          //  BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
