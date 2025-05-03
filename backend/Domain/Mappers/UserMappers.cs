using Data.Models;
using Domain.DTOs.User;

public static class UserMappers
{
    public static UserDto ToUserDto(this User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            DateJoined = user.CreatedAt
        };
    }
}