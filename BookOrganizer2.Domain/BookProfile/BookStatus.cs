using System;

namespace BookOrganizer2.Domain.BookProfile
{
    [Flags]
    public enum BookStatus
    {
        None = 0,
        Read = 1,
        Owned = 2,

        OwnedAndRead = Read | Owned
    }
}
