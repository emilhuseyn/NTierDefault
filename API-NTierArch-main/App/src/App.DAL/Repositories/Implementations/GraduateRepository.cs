using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Entities;
using App.DAL.Presistence;
using App.DAL.Repositories.Abstractions;
using App.DAL.Repositories.Interfaces;

namespace App.DAL.Repositories.Implementations
{
    public class GraduateRepository : Repository<Graduate>, IGraduateRepository
    {
        public GraduateRepository(AppDbContext context) : base(context)
        {
        }
    }
}
