namespace MonolithBlazorApp.UI;

public class HeroViewModel
{  
  public required Guid Id { get; set; }
  public required string HeroName { get; set; }
  public required string Power { get; set; }
  public required string Email { get; set; }
}