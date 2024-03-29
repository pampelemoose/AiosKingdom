﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DataModels
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        [EmailAddress, ConcurrencyCheck]
        public string Email { get; set; }

        public string InternalRoles { get; set; }
        public List<String> Roles
        {
            get
            {
                var result = new List<Role>();
                return InternalRoles.Split(';').ToList();
            }
            set
            {
                InternalRoles = String.Join(";", value);
            }
        }

        public Guid ActivationCode { get; set; }

        public bool IsActivated { get; set; }

        public static string EncryptPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
