﻿using CPEFinalProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPEFinalProject.Services.Interfaces
{
    public interface IAuthenticationService
    {
        void Authenticate(ApplicationUserTypeEnum userType);
    }
}
