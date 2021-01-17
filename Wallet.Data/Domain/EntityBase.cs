using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Data.Domain
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsNew() => Id == 0;
    }
}
