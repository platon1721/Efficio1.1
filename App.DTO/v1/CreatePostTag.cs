namespace App.DTO.v1;

public class CreatePostTag
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
}