﻿using System.ComponentModel.DataAnnotations;

namespace Catalog.Models;

public class BrandModel : IValidatableObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Descr { get; set; }
    [Url]
    public string LogoURL { get; set; } = "https://developer.mozilla.org";
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        
        if(string.IsNullOrWhiteSpace(Name))
            errors.Add(new ValidationResult("Brand name couldn't be empty, null, or contains only whitespaces", new List<string>() {"Brand"}));
        if(string.IsNullOrWhiteSpace(Descr))
            errors.Add(new ValidationResult("Description couldn't be empty, null, or contains only whitespaces", new List<string>() {"Descr"}));

        return errors;
    }
}