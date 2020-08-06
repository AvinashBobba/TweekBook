using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweekBook.Data;
using TweekBook.Domain;
using TweekBook.Options;

namespace TweekBook.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly JWTSettings _jWTSettings;

        private readonly TokenValidationParameters _tokenValidationParameters;

        private readonly DataContext _dataContext;

        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(
            UserManager<IdentityUser> userManager,
            JWTSettings jWTSettings,
            TokenValidationParameters tokenValidationParameters,
            RoleManager<IdentityRole> roleManager,
            DataContext dataContext)
        {
            ;
            _userManager = userManager;
            _jWTSettings = jWTSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _dataContext = dataContext;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "User Doesnot Exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user,password);
            if(!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new [] {"User/Password combination is wrong"}
                };
            }

            return await GenerateAuthenticationResultForUser(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if(validatedToken == null)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new [] {"InValid Token"}
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateUtc = new DateTime(year:1970,month:1,day:1,hour:0,minute:0,second:0,DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if(expiryDateUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "This Token Hasn't expired yet" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if(storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "This Refresh Token Doesn't exist" }
                };
            }

            if(DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "The refresh Token has expired" }
                };
            }

            //if(storedRefreshToken.InValidated)
            //{
            //    return new AuthenticationResult
            //    {
            //        ErrorMessage = new[] { "This refresh Token has been Invalidated" }
            //    };
            //}

            if(storedRefreshToken.Used)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "This Refresh Token Has been Used" }
                };
            }

            if(storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "This Refresh Token doesnot match the JWT" }
                };
            }

            storedRefreshToken.Used = true;
            _dataContext.RefreshTokens.Update(storedRefreshToken);
            await _dataContext.SaveChangesAsync();
            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUser(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken securityToken)
        {
            return (securityToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = new[] { "User with this mail already exists" }
                };
            }

            var newUserId = Guid.NewGuid();
            var newUser = new IdentityUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    ErrorMessage = createdUser.Errors.Select(x => x.Description)
                };
            }

            // Endpoints restricted and adding claim policy manually 

            //await _userManager.AddToRoleAsync(newUser, "Modator");
            //await _userManager.AddClaimAsync(newUser, new Claim(type: "tags.view", value: "true"));
            return await GenerateAuthenticationResultForUser(newUser);
        }



        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jWTSettings.Secret);

            //Adding the Claims to the token

            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim("id",user.Id)
                };

            var userClaims = await _userManager.GetClaimsAsync(user);

            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jWTSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _dataContext.RefreshTokens.AddAsync(refreshToken);

            await _dataContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}
