using System;

namespace Games4Trade.Dtos
{
    public class AdvertisementSaveDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Discriminator { get; set; }
        public bool ExchangeActive {get;set;}
        public decimal Price { get; set; }
        public bool ShowUserEmail { get; set; }
        public bool ShowUserPhoneNumber { get; set; }
        public int UserId { get; set; }
        public int StateId { get; set; }
        public int SystemId { get; set; }
        // game & console
        public int RegionId { get; set; }
        //game part
        public DateTime DateDeveloped { get; set; }
        public string Developer { get; set; }
        public string GenreId { get; set; }
        //console & accessory part
        public DateTime DateManufactured { get; set; }
        //accessory part
        public string AccessoryManufacturer { get; set; }
        public string AccessoryModel { get; set; }
    }
}
