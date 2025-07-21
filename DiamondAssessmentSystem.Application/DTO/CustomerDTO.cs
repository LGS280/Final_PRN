using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class CustomerDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Gender { get; set; }
        public int? Point { get; set; }
        public string? Note { get; set; }
        public string? Phone { get; set; }
        public string? IdCard { get; set; }
        public string? Address { get; set; }
        public string? UnitName { get; set; }
        public string? TaxCode { get; set; }
        public AccountDto? Acc { get; set; }
    }

    public class CustomerCreateDto
    {
        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [RegularExpression("^(Male|Female|Other)?$", ErrorMessage = "Gender must be Male, Female, or Other.")]
        public string? Gender { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        public string? Phone { get; set; }

        [RegularExpression(@"^\d{9,12}$", ErrorMessage = "ID Card must be numeric and 9–12 digits.")]
        public string? IdCard { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? UnitName { get; set; }

        [StringLength(20)]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Tax Code must be 10–15 digits.")]
        public string? TaxCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

    }

    public class CustomerUpdateDto
    {
        public string UserId { get; set; }
        public CustomerCreateDto Customer { get; set; }
    }

    public class CustomerUpdateDtoVer1
    {
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? IdCard { get; set; }
        public string? Address { get; set; }
        public string? UnitName { get; set; }
        public string? TaxCode { get; set; }
        public string? Email { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
