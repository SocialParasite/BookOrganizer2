using System;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.Exceptions;

namespace BookOrganizer2.Domain
{
    public class Author
    {
        public AuthorId Id { get; private set; }
        public string FirstName { get; private set; }

        public void SetFirstName(string name)
        {
            if(ValidateName(name))
                FirstName = name;
            else 
                throw new InvalidFirstNameException();
        }

        private static bool ValidateName(string name)
        {
            if (name == null)
                throw new InvalidFirstNameException();

            var regexPattern = new Regex("^(?=.{1,64}$)[A-Za-z]+(?:[ '-][A-Za-z]+)?$");

            return regexPattern.IsMatch(name);   
        }
    }
}
