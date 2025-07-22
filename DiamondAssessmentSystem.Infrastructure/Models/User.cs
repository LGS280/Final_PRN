using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Models
{
    public class User : IdentityUser
    {
        public string UserType { get; set; } = null!; // Customer/Employee

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Gender { get; set; }

        public int? Point { get; set; }

        public DateTime? DateCreated { get; set; }

        public string? Note { get; set; }

        public string? Status { get; set; }

        public virtual Customer? Customer { get; set; }

        public virtual Employee? Employee { get; set; }

    }
}
