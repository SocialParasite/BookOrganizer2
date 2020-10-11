using System;
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
            MugshotPath = pic;
        }

        private static bool ValidateName(string name, Action exception)
        {
            if (string.IsNullOrWhiteSpace(name)) 
                exception.Invoke();

            var regexPattern = new Regex("(?=.{1,64}$)^[\\p{L}\\p{M}\\s'-]+?$");

            return regexPattern.IsMatch(name ?? string.Empty);   
        }
    }
}
