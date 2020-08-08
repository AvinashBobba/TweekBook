using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Requests;
using TweekBook.Contracts.V1.Responses;

namespace TweekBook.SDK
{
    public interface IIdentityApi
    {
        [Post(path: "/api/v1/identity/register")]
        Task<ApiResponse<AuthSuccessResponse>> RegisterAsync([Body] UserRegisterationRequest userRegisterationRequest);

        [Post(path: "/api/v1/identity/login")]
        Task<ApiResponse<AuthSuccessResponse>> LoginAsync([Body] UserLoginRequest userLoginRequest);

        [Post(path: "/api/v1/identity/refresh")]
        Task<ApiResponse<AuthSuccessResponse>> RefreshAsync([Body] RefreshTokenRequest refreshTokenRequest);
    }
}
