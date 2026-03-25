using Shared.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Identity
{
    public interface ITokenService
    {
        TokenResponse GetToken(TokenRequest request);
    }
}
