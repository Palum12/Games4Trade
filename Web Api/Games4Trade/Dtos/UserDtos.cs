using System.Collections.Generic;

namespace Games4TradeAPI.Dtos
{
    public class UserSimpleDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
    }

    public class UserDto : UserSimpleDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
    }

    public class UserLikedGenresDto
    {
        public IList<int> GenresIds { get; set; }
    }
    public class UserOwnedSystemsDto
    {
        public IList<int> SystemsIds { get; set; }
    }

    public class ObservedUserDto
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public IList<string> LikedGenres { get; set; }
        public IList<string> InterestingSystems { get; set; }
    } 

    public class UserProfileDto : ObservedUserDto
    {
        public bool? IsUserObserved { get; set; }
    }

    public class UserLoginDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterDto
    {
        public string Login { get; set; }
        public string Email { get; set; }
    }

    public class UserRecoverDto
    {
        public string RecoveryString { get; set; }
        public string NewPassword { get; set; }
    }
}
