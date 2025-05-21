using System.ComponentModel.DataAnnotations;
using Base.Domain;


namespace WebApp.ViewModels;

public class ContactTypeEditViewModel : BaseEntity
{
    [MaxLength(128)]
    public string ContactTypeName { get; set; } = default!;

}