namespace BusinessLayer.DTO;

public class UserReadDto : BaseUserDto
{
    public Guid Id { get; set; }
    public List<RoleDto> Roles { get; set; }
}