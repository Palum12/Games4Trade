using System;
using System.Collections.Generic;

namespace Games4TradeAPI.Models
{
    public class Advertisement : ModelBase
    {
        public DateTime DateCreated { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool? ExchangeActive { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public AdvertisementItem Item { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
