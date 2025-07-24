using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Auth
{
    public class ApplicationRole : IdentityRole
    {
        // Bạn có thể thêm các thuộc tính tùy chỉnh cho vai trò ở đây nếu cần
        public string Description { get; set; }  // Mô tả vai trò, ví dụ "Quản lý hệ thống"
        public DateTime CreatedDate { get; set; } // Ngày tạo vai trò
    }
}
