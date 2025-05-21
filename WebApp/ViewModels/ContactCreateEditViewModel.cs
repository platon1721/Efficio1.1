using App.BLL.DTO;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.ViewModels;

public class ContactCreateEditViewModel
{
    public Contact Contact { get; set; } = default!;

    [ValidateNever]
    public SelectList PersonSelectList { get; set; } = default!;

    [ValidateNever]
    public SelectList ContactTypeSelectList { get; set; } = default!;
}