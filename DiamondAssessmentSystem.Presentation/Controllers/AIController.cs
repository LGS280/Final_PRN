using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly IServicePriceService _servicePriceService;

        public AIController(IAIService aiService, IServicePriceService servicePriceService)
        {
            _aiService = aiService;
            _servicePriceService = servicePriceService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] string input)
        {
            if (string.IsNullOrWhiteSpace(input) || !input.Trim().StartsWith("@Gemini", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid input");

            var query = input.Trim().Substring(7).Trim();
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Empty query");

            if (IsPriceQuery(query))
            {
                var servicePrices = await _servicePriceService.GetByStatusAsync("Active");

                if (!servicePrices.Any())
                    return Ok("Hiện tại không có dịch vụ nào được cung cấp.");

                var sb = new StringBuilder();
                sb.AppendLine("Danh sách dịch vụ hiện có của hệ thống giám định kim cương:");
                foreach (var item in servicePrices)
                {
                    sb.AppendLine($"- {item.ServiceType}: {item.Price:N0} VND ({item.Description ?? "Không mô tả"})");
                }

                sb.AppendLine($"\nHãy trả lời ngắn gọn cho người dùng dựa trên danh sách trên: {query}");

                query = sb.ToString();
            }

            var result = await _aiService.AskGeminiAsync(query);
            return Ok(result);
        }

        private bool IsPriceQuery(string input)
        {
            var lowered = input.ToLowerInvariant();
            return lowered.Contains("giá") || lowered.Contains("bao nhiêu") || lowered.Contains("price") || lowered.Contains("phí");
        }
    }
}
