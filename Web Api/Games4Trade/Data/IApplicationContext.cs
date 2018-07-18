using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Data
{
    public interface IApplicationContext
    {
        DbSet<User> Users { get; set; }
    }
}
