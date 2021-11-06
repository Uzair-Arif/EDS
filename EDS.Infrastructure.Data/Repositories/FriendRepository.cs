using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EDS.Domain.Interfaces;
using EDS.Domain.Models;
using EM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EDS.Infrastructure.Data.Repositories
{
    public class FriendRepository : GenericRepository<Friend>, IFriendRepository
    {
       
    }
}
