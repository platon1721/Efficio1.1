using System.ComponentModel.DataAnnotations;
using Base.Domain;


namespace WebApp.ViewModels;

public class ContactTypeCreateViewModel
{
    [MaxLength(128)]
    public string ContactTypeName { get; set; } = default!;
}