﻿using Core.DataAccess;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public  interface ICompanyDal:IEntityRepository<Company>
    {
        void UserCompanyAdd(int userId, int companyId); 
    }
}