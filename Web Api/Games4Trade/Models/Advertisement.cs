using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class Advertisement
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool? ExchangeActive { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public AdvertisementItem Item { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
