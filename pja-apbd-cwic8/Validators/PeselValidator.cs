using System.ComponentModel.DataAnnotations;

namespace pja_apbd_cwic8.Validators;

public class PeselValidator
{
    private static readonly HashSet<char> _numbers = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public static ValidationResult ValidatePesel(string s)
    {
        if (s.Length != 11) return new ValidationResult("Wrong length");

        foreach (var c in s)
            if (!_numbers.Contains(c))
                return new ValidationResult("Not a number");

        return ValidationResult.Success;
    }
}