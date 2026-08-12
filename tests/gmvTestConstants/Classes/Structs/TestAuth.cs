using System;
using System.Text;

namespace gmvTestConstants
{
    public readonly struct TestAuth
    {
        public string Username => "ladot";
        public string Password => "dieengele";
        public string BasicAuthHeader => $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"))}";
    }
}
