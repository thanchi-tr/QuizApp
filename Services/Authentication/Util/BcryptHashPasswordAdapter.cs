﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace QuizApp.Services.Authentication.Util
{
    public class BcryptHashPasswordAdapter : IPasswordVerificate, IPasswordHash
    {
        /// <summary>
        /// Allow for flexibility in swap out module
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Allow for flexibility in switching verification method
        /// </summary>
        /// <param name="password"></param>
        /// <param name="authenticatedPassword"></param>
        /// <returns></returns>
        public bool Verify(string password, string authenticatedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, authenticatedPassword);
        }
    }
}