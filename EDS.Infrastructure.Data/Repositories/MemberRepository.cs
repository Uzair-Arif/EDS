using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EDS.Domain.Interfaces;
using EDS.Domain.Models;
using EM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EDS.Infrastructure.Data.Repositories
{
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        
    }
}
