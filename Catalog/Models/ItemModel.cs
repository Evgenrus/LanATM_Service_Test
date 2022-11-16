using System.ComponentModel.DataAnnotations;

namespace Catalog.Models;

public class ItemModel : IValidatableObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Article { get; set; }
    public string Descr { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int Stock { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add(new ValidationResult("Name couldn't be empty, null, or consists only whitespaces", new List<string>() {"Name"}));
        if (string.IsNullOrWhiteSpace(Article))
            errors.Add(new ValidationResult("Article couldn't be empty, null, or consists only whitespaces", new List<string>() {"Article"}));
        if (string.IsNullOrWhiteSpace(Descr))
            errors.Add(new ValidationResult("Description couldn't be empty, null, or consists only whitespaces", new List<string>() {"Descr"}));
        if (string.IsNullOrWhiteSpace(Brand))
            errors.Add(new ValidationResult("Brand name couldn't be empty, null, or consists only whitespaces", new List<string>() {"Brand"}));
        if (string.IsNullOrWhiteSpace(Category))
            errors.Add(new ValidationResult("Category couldn't be empty, null, or consists only whitespaces", new List<string>() {"Category"}));
        if(Stock < 0 || Stock > 500)
            errors.Add(new ValidationResult("Stock must be between 0 and 500", new List<string>() {"Stock"}));

        return errors;
    }
}