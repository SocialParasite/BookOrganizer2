using System;
using System.Text.RegularExpressions;
using BookOrganizer2.Domain.Exceptions;

namespace BookOrganizer2.Domain
{
    public class Author
    {
        public AuthorId Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        
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

        private static bool ValidateName(string name, Action exception)
        {
            if (name == null) 
                exception.Invoke();

            var regexPattern = new Regex("^(?=.{1,64}$)[A-Za-z]+(?:[ '-][A-Za-z]+)?$");

            return regexPattern.IsMatch(name ?? string.Empty);   
        }
    }
}
