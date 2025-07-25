using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Enums
{
    public enum CustomerEnum
    {
    }

    public enum UpdateCustomerResult
    {
        Success,
        CustomerNotFound,
        InvalidPhoneNumber,
        UpdateFailed
    }
}
