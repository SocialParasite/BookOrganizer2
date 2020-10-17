using BookOrganizer2.Domain.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BookOrganizer2.Domain
{
    public class Author
    {
        private Author() { }

        public AuthorId Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string Biography { get; private set; }
        public string MugshotPath { get; private set; }
        public string Notes { get; private set; }

        public static Author Create(AuthorId id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id), "Store without unique identifier cannot be created.");

            var author = new Author
            {
                Id = id
            };

            return author;
        }

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

        public void SetNotes(string notes)
        {
            Notes = notes;
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
            var pattern = "(?=.{" + minLength + "," + maxLength +"}$)^[\\p{L}\\p{M}\\s'-]+?$";

            if (string.IsNullOrWhiteSpace(name)) 
                exception.Invoke();

            var regexPattern = new Regex(pattern);

            return regexPattern.IsMatch(name ?? string.Empty);   
        }
    }
}
