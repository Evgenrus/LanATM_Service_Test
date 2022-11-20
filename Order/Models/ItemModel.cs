using System.ComponentModel.DataAnnotations;

namespace Order.Models;

public class ItemModel : IValidatableObject
{
    public string Name { get; set; }
    public string Article { get; set; }
    public string Descr { get; set; }
    public string Brand { get; set; }
    public string Category { get; set; }
    public int Count { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if(string.IsNullOrWhiteSpace(Name))
            errors.Add(new ValidationResult(
                "Name shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Name"})
            );
        if(string.IsNullOrWhiteSpace(Article))
            errors.Add(new ValidationResult(
                "Article shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Article"})
            );
        if(string.IsNullOrWhiteSpace(Descr))
            errors.Add(new ValidationResult(
                "Name shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Descr"})
            );
        if(string.IsNullOrWhiteSpace(Brand))
            errors.Add(new ValidationResult(
                "Brand shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Brand"})
            );
        if(string.IsNullOrWhiteSpace(Category))
            errors.Add(new ValidationResult(
                "Category shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Category"})
            );
        if(Count < 1 || Count > 500)
            errors.Add(new ValidationResult(
                "Count should be in range from 1 to 500", 
                new List<string>() {"Count"})
            );

        return errors;
    }
}