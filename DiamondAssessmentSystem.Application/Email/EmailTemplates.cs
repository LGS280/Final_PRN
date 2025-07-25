using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Email
{
    public static class EmailTemplates
    {
        public static string ConfirmEmailTemplate(string confirmationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Confirm your email</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
        }}

        .container {{
            padding: 20px;
            background-color: #f9f9f9;
        }}

        .btn {{
            background-color: #007bff;
            color: white;
            padding: 10px 15px;
            text-decoration: none;
            border-radius: 5px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Welcome to Our Service!</h2>
        <p>Please confirm your email by clicking the button below:</p>
        <a href=""{confirmationLink}"" class=""btn"">Confirm Email</a>
        <p>If you did not create this account, please ignore this email.</p>
    </div>
</body>
</html>";
        }
    }
}
