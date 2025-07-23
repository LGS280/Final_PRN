using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IAIService
    {
        Task<string> AskGeminiAsync(string prompt);
    }
}
