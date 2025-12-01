using Microsoft.AspNetCore.Components;
using MonolithBlazorApp.Infrastructure;
using OtherDomain.EFCore.Entities;

namespace MonolithBlazorApp.UI.Components.Pages;

public partial class Heroes
{
  private string? _errorMessage;

  [Inject]
  public required NavigationManager Navigation { get; init; }

  [Inject]
  public required OtherDomainODataServiceContext OtherDomainClient { get; init; }

  // TODO: Set properties to hero api client and data
  private List<HeroViewModel>? _heroes;
  private string? SearchPattern { get; set; }

  // TODO: Set entity linked to the create form
  [SupplyParameterFromForm]
  private HeroFormViewModel HeroFormViewModel { get; set; } = new HeroFormViewModel()
  {
    Power = string.Empty,
    HeroName = string.Empty,
    Email = string.Empty
  };

  protected override async Task OnInitializedAsync()
  {
    try
    {
      // TODO: Initialize Heroes list
      await SearchHeroesAsync();
    }
    catch (Exception ex)
    {
      _errorMessage = $"Error during hero initialization: {ex.Message}";
    }
  }

  private void ReloadPage()
  {
    Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
  }

  // TODO: Search method
  protected async Task SearchHeroesAsync(string? searchPattern = null)
  {
    try
    {
      // TODO : to improve
      _heroes = (await OtherDomainClient
        .SuperHeroes        
        .ExecuteAsync())
        .Select(item => new HeroViewModel
        {
          Id = item.Id,
          HeroName = item.Name,
          Power = "No Power",
          Email = "NoEmail@email.com",
        })
        .ToList();

      StateHasChanged();
    }
    catch (Exception ex)
    {
      _errorMessage = $"Error during hero search: {ex.Message}";
    }
  }

  // TODO: Create method
  protected async Task CreateAsync()
  {
    try
    {
      // To improve
      OtherDomainClient.AddObject("SuperHeroes", new SuperHero
      {
        Name = HeroFormViewModel.HeroName
      });
      var response = await OtherDomainClient.SaveChangesAsync();      

      await SearchHeroesAsync();
      StateHasChanged();
    }
    catch (Exception ex)
    {
      _errorMessage = $"Error during hero creation: {ex.Message}";
    }
  }

  // TODO: Modal dialog to display details
  private bool _isHeroDialogOpen = false;

  private async Task OpenHeroDialogAsync(HeroViewModel hero)
  {
    try
    {
      _isHeroDialogOpen = true;
 
      StateHasChanged();
    }
    catch (Exception ex)
    {
      _errorMessage = $"Error during hero details: {ex.Message}";
    }
  }

  private void CloseHeroDialog()
  {
    _isHeroDialogOpen = false;
  }
}
