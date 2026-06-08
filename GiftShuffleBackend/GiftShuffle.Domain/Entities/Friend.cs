using System;
using System.Collections.Generic;
using System.Text;

namespace GiftShuffle.Domain.Entities
{
    public class Friend
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}