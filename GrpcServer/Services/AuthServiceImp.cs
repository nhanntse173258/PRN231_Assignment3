using Grpc.Core;
using GrpcServer.Repository;
using GrpcServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrpcServer.Services
{
    public class AuthServiceImpl : AuthService.AuthServiceBase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtSettings _jwtSettings; // Injected JwtSettings

        public AuthServiceImpl(UserRepository userRepository, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings;
        }

        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var user = _userRepository.GetUserByUsername(request.Username);

            if (user == null || user.Password != request.Password) // Implement proper password hashing
            {
                //throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid username or password"));
                return Task.FromResult(new LoginResponse
                {
                    Token = string.Empty // or `null` if you prefer
                });
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            return Task.FromResult(new LoginResponse { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTime);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public override Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero // Optional: to eliminate delay
                }, out var validatedToken);

                // If token is valid, extract user ID and return details
                var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Task.FromResult(new ValidateTokenResponse
                {
                    IsValid = true,
                    UserId = userId
                });
            }
            catch (Exception)
            {
                // Token is invalid or expired
                return Task.FromResult(new ValidateTokenResponse { IsValid = false });
            }
        }

        // Logout implementation
        public override Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
        {
            // JWT tokens are stateless, typically logouts are handled differently
            return Task.FromResult(new LogoutResponse { Success = true });
        }
    }
}

