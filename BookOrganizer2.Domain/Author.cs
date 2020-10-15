using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.Exceptions;

namespace BookOrganizer2.Domain
{
    public class Author
    {
        public AuthorId Id { get; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string Biography { get; private set; }
        public string MugshotPath { get; private set; }

        public void SetFirstName(string name)
        {
            if(ValidateName(name, () => throw new InvalidFirstNameException()))
                FirstName = name;
            else 
                throw new InvalidFirstNameException();
        }

        public void SetLastName(string name)
        {
            if (ValidateName(name, () => throw new InvalidLastNameException()))
                LastName = name;
            else
                throw new InvalidLastNameException();
        }

        public void SetDateOfBirth(DateTime? dob)
        {
            DateOfBirth = dob;
        }

        public void SetBiography(string bio)
        {
            Biography = bio;
        }

        public void SetMugshotPath(string pic)
        {
            var path = Path.GetFullPath(pic);
            string[] formats = { ".jpg", ".png", ".gif", ".jpeg" };

            if (formats.Contains(Path.GetExtension(pic), StringComparer.InvariantCultureIgnoreCase))
                MugshotPath = path;
            else 
                throw new Exception();
        }

        private static bool ValidateName(string name, Action exception)
        {
            const int minLength = 1;
            const int maxLength = 64;
            string pattern = "(?=.{" + minLength + "," + maxLength +"}$)^[\\p{L}\\p{M}\\s'-]+?$";

            if (string.IsNullOrWhiteSpace(name)) 
                exception.Invoke();

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name ?? string.Empty);   
        }
    }
}
