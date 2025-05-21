using App.BLL.DTO;


namespace WebApp.ViewModels;

public class PersonIndexViewModel
{
    public ICollection<Person> Persons { get; set; } = default!;

    public int PersonCountByName { get; set; }
}