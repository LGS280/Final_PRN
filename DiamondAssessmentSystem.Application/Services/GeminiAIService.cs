using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class GeminiAIService : IAIService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public GeminiAIService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<string> AskGeminiAsync(string prompt)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = $"Trả lời ngắn gọn dưới 200 từ bằng tiếng Việt:\n{prompt}" }
                    }
                }
            }
            };

            var response = await _httpClient.PostAsync(endpoint,
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(json);

            return root.RootElement
                       .GetProperty("candidates")[0]
                       .GetProperty("content")
                       .GetProperty("parts")[0]
                       .GetProperty("text")
                       .GetString() ?? "Không thể trả lời.";
        }
    }
}
