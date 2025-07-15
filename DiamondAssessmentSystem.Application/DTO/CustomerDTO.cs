namespace DiamondAssessmentSystem.Application.DTO
{
    public class CustomerDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
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
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? IdCard { get; set; }
        public string? Address { get; set; }
        public string? UnitName { get; set; }
        public string? TaxCode { get; set; }
    }
}
