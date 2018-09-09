using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Games4Trade.Models
{
    public class Accessory : AdvertisementItem
    {
        public string AccessoryManufacturer { get; set; }
        public string AccessoryModel { get; set; }
    }
}
