using System;

namespace Games4TradeAPI.Models
{
    public class Photo : ModelBase
    {
        public string FileName { get; set; }
        public string ObjectName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool HasMiniature { get; set; }
        public byte[] Bytes { get; set; }
        public int? AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
    }
}
