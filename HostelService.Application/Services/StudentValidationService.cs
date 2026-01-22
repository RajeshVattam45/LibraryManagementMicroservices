using HostelService.Application.DTOs;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HostelService.Application.Services
{
    public class StudentValidationService : IStudentValidationService
    {
        private readonly HttpClient _http;

        private string? _cachedToken;
        private DateTime _tokenExpiry;

        public StudentValidationService ( HttpClient http )
        {
            _http = http;
        }

        // Automatically login + store token
        private async Task AuthenticateAsync ( )
        {
            // If we have a valid token, don't login again
            if (_cachedToken != null && DateTime.UtcNow < _tokenExpiry)
                return;

            var loginBody = new
            {
                email = "admin@admin.com",
                password = "admin"
            };

            var loginResponse = await _http.PostAsJsonAsync ( "api/auth/login", loginBody );

            if (!loginResponse.IsSuccessStatusCode)
                throw new Exception ( "Failed to authenticate with Student API." );

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse> ();

            if (loginResult == null || string.IsNullOrEmpty ( loginResult.Token ))
                throw new Exception ( "Student API returned an invalid token." );

            _cachedToken = loginResult.Token;

            // Assume token valid for 1 hour (you can parse JWT expiry if needed)
            _tokenExpiry = DateTime.UtcNow.AddHours ( 1 );

            // Add Authorization header for ALL future requests
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue ( "Bearer", _cachedToken );
        }

        // Validate student existence
        public async Task<bool> StudentExistsAsync ( int studentId )
        {
            // Ensure token is refreshed before making request
            await AuthenticateAsync ();

            var response = await _http.GetAsync (
                $"api/students/check-studentid-exists/{studentId}" );

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<StudentExistsResponse> ();

            return result?.Exists ?? false;
        }

        // DTO mapping model
        private class LoginResponse
        {
            public string Token { get; set; }
        }
    }
}
