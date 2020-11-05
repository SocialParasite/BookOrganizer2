using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookOrganizer2.Domain.Shared;

namespace BookOrganizer2.Domain.AuthorProfile
{
    public class Nationality : IIdentifiable<NationalityId>
    {
        public NationalityId Id { get; set; }

        public string Name { get; set; }

        // Navigational properties
        public ICollection<Author> Authors { get; set; }
    }
}