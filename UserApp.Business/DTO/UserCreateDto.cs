namespace BusinessLayer.DTO;

public class UserCreateDto : BaseUserDto
{
    public List<int> Roles { get; set; }
}