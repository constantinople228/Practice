namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<ProductVersion> Versions { get; set; } = new List<ProductVersion>();
}