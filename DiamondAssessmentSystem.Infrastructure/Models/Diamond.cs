using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Models
{
    public partial class Diamond
    {
        public int DiamondId { get; set; }

        public DateTime? DateReceived { get; set; }

        public DateTime? DateReturn { get; set; }

        public int? EmployeeId { get; set; }

        public int RequestId { get; set; }

        public string? Status { get; set; }

        public virtual Employee? Employee { get; set; }

        public virtual Request Request { get; set; } = null!;

        public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    }
}
