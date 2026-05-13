using System.ComponentModel.DataAnnotations;

namespace Suprema.Web.ViewModels;

public sealed class ContactFormViewModel
{
    [Required, MaxLength(100)]
    public string? Name { get; set; }

    [Required, EmailAddress, MaxLength(200)]
    public string? Email { get; set; }

    [Phone, MaxLength(30)]
    public string? Phone { get; set; }

    [Required, MaxLength(2000)]
    public string? Message { get; set; }
}
