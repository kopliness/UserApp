namespace BusinessLayer.DTO;

public class UserRoleDto
{
    public Guid UserId { get; set; }
    public List<int> RoleIds { get; set; }
}