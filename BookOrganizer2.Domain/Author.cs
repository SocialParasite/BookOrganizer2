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

        private bool ValidateName(string name)
        {
            return name != null
                && name.Length <= 64 
                && name.Length > 0;   
        }
    }
}
