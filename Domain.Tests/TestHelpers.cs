using System.ComponentModel.DataAnnotations;

namespace Domain.Tests;

public class TestHelpers
{
    // Hjälpmetod som tar emot ett objekt och kollar ifall den uppfyller alla sina valideringar som t.ex "Required", "MaxLength"
    public static bool IsValid(object obj)
    {
        var context = new ValidationContext(obj);
        return Validator.TryValidateObject(obj, context, null, validateAllProperties: true);
    }
}