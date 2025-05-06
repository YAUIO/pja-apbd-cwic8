using System.ComponentModel.DataAnnotations;
using pja_apbd_cwic8.Validators;

namespace pja_apbd_cwic8.DTOs;

public class ClientPost
{
    [Required] public string FirstName { set; get; }

    [Required] public string LastName { set; get; }

    [EmailAddress] [Required] public string Email { set; get; }

    [Required] [Phone] public string Telephone { set; get; }

    [Required]
    [CustomValidation(typeof(PeselValidator), "ValidatePesel")]
    public string Pesel { set; get; }
}