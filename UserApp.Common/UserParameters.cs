namespace UserApp.Common;

public class UserParameters
{
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = value;
        }
    }
    public string? OrderBy { get; set; }
    public string? Name { get; set; }
    
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public string? Email { get; set; }
    public string? RoleName { get; set; }
}