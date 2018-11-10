using System;
using System.Collections.Generic;
using FluentValidation;
using Games4Trade.Dtos;


namespace Games4Trade.Validators
{
    public class AdvertisementSaveValidator : AbstractValidator<AdvertisementSaveDto>
    {
        public AdvertisementSaveValidator()
        {
            var discriminators = new List<string> {"Game", "Console", "Accessory"};

            RuleFor(a => a.Discriminator).NotNull().Must(a => discriminators.Contains(a));
            RuleFor(a => a.Description).NotNull().NotEmpty();
            RuleFor(a => a.ExchangeActive).NotNull();
            RuleFor(a => a.Title).NotNull();
            RuleFor(a => a.Price).GreaterThan(0);        
            RuleFor(a => a.AccessoryManufacturer)
                .NotNull()
                .When(a => a.Discriminator.Equals("Accessory"));
            RuleFor(a => a.AccessoryModel)
                .NotNull()
                .When(a => a.Discriminator.Equals("Accessory"));
            RuleFor(a => a.DateReleased)
                .GreaterThan(DateTime.Parse("1960-01-01"))
                .LessThan(DateTime.Today.AddYears(1))
                .When(a => a.DateReleased != null);


            RuleFor(a => a.StateId).NotNull().GreaterThan(0);
            RuleFor(a => a.SystemId).NotNull().GreaterThan(0);
            RuleFor(a => a.RegionId).NotNull().Unless(a => a.Discriminator.Equals("Accessory"));
            RuleFor(a => a.GenreId).NotNull().When(a => a.Discriminator.Equals("Game"));
        }
    }
}
