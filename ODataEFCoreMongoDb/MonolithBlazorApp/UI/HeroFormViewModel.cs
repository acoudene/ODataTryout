using System.ComponentModel.DataAnnotations;

namespace MonolithBlazorApp.UI;

public class HeroFormViewModel
{

  [Required(ErrorMessage = "Hero name is mandatory.")]
  [RegularExpression("^[a-zA-Z0-9\\-]{2,50}$", 
    ErrorMessage = "Name must contain characters, numbers or hyphens (2 to 50 characters).")]
  public required string HeroName { get; set; }

  [Required(ErrorMessage = "Power is mandatory to be a super hero.")]
  [StringLength(100, ErrorMessage = "Power can't exceed 100 characters.")]
  public required string Power { get; set; }

  [Required(ErrorMessage = "Email is mandatory.")]
  [EmailAddress(ErrorMessage = "Invalid Email address.")]
  public required string Email { get; set; }

}
