﻿using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserDTOForRegistration userDTOForRegistration);
        Task<bool> ValidateUser(UserDTOForAuthentication userDTOForAuthentication);
        Task<string> CreateToken();
    }
}
