namespace UserApp.Common;

public class UserParameters
{
    const int maxPageSize = 50;
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
            if (value > maxPageSize)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"PageSize cannot be more than {maxPageSize}");
            }
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


