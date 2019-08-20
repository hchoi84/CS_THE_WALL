using System.ComponentModel.DataAnnotations;

namespace _01_THE_WALL.Models
{
  public class LoginModel
  {
    [Required]
    [Display(Name="Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
}