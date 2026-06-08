using System;
using System.Collections.Generic;
using System.Text;

namespace GiftShuffle.Domain.Entities
{
    public class ShuffleHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GiverFriendId { get; set; }
        public Guid ReceiverFriendId { get; set; }
        public DateTime ShuffleDate { get; set; } = DateTime.UtcNow;
    }
}
