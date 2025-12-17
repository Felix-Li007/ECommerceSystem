namespace AdminWeb.Models.DTOs;

public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}