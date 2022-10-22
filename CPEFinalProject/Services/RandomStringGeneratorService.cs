using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services
{
    public static class RandomStringGeneratorService
    {
        private static string sourceString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateRandomString(int size)
        {
            Random rand = new Random();
            string newString = string.Empty;

            do
            {
                newString += sourceString[rand.Next(sourceString.Length-1)];
            } while (newString.Length < size);

            return newString;
        }
    }
}
