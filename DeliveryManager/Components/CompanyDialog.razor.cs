using System.ComponentModel.DataAnnotations;
using DeliveryManager.Interfaces;
using DeliveryManager.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeliveryManager.Components;

public partial class CompanyDialogBase : ComponentBase
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public Company? Company { get; set; }

    [Inject] private ICompanyService CompanyService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    public MudForm form = null!;
    public bool isValid;
    public bool isTouched;
    public CompanyModel model = new();

    protected bool IsEditMode => Company != null;

    // MudForm reports a form with required fields as invalid until it is touched, so unchanged existing data stays saveable
    protected bool CanSave => isValid || (IsEditMode && !isTouched);

    protected override void OnInitialized()
    {
        if (Company is null) return;

        model = new CompanyModel
        {
            Name = Company.Name,
            ShortName = Company.ShortName,
            Address = Company.Address,
            Email = Company.Email
        };
    }

    public void Cancel() => MudDialog.Cancel();

    public async Task Save()
    {
        await form.Validate();
        if (!form.IsValid)
        {
            Snackbar.Add("Validációs hibák jelentkeztek", Severity.Error);
            return;
        }

        try
        {
            if (IsEditMode && Company != null)
            {
                Company.Name = model.Name;
                Company.ShortName = model.ShortName;
                Company.Address = model.Address;
                Company.Email = model.Email;

                await CompanyService.UpdateAsync(Company);
                Snackbar.Add("Cég sikeresen mentve", Severity.Success);
            }
            else
            {
                var newCompany = new Company
                {
                    Name = model.Name,
                    ShortName = model.ShortName,
                    Address = model.Address,
                    Email = model.Email
                };

                await CompanyService.CreateAsync(newCompany);
                Snackbar.Add("Cég sikeresen létrehozva", Severity.Success);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Mentési hiba: {ex.Message}", Severity.Error);
        }
    }

    public class CompanyModel
    {
        [Required(ErrorMessage = "Név kötelező")]
        [StringLength(200, ErrorMessage = "Név nem lehet 200 karakternél hosszabb")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Rövid név nem lehet 50 karakternél hosszabb")]
        public string ShortName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Cím nem lehet 500 karakternél hosszabb")]
        public string Address { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email formátum hibás")]
        [StringLength(200, ErrorMessage = "Email nem lehet 200 karakternél hosszabb")]
        public string Email { get; set; } = string.Empty;
    }
}