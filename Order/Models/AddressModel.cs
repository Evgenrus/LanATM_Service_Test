using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Order.Models;

public class AddressModel : IValidatableObject
{
    public int? Id { get; set; }
    public string Region { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Street { get; set; }
    public string House { get; set; }
    public int? Floor { get; set; }
    public string? Flat { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if(string.IsNullOrWhiteSpace(Region))
            errors.Add(new ValidationResult(
                "Region shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Region"})
            );
        if(string.IsNullOrWhiteSpace(City))
            errors.Add(new ValidationResult(
                "City shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"City"})
            );
        if(string.IsNullOrWhiteSpace(District))
            errors.Add(new ValidationResult(
                "District shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"District"})
            );
        if(string.IsNullOrWhiteSpace(Street))
            errors.Add(new ValidationResult(
                "Street shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"Street"})
            );
        if(string.IsNullOrWhiteSpace(House))
            errors.Add(new ValidationResult(
                "House shouldn't be empty, null or contains only whitespaces", 
                new List<string>() {"House"})
            );
        return errors;
    }
}