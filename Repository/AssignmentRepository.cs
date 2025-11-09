using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Assignment;
using API.Entities;
using API.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;
        public AssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Assignment> CreateAsync(Assignment assignmentModel)
        {
            await _context.Assignments.AddAsync(assignmentModel);
            return assignmentModel;
        }

        public async Task<Assignment?> GetByIdAsync(int assignmentId)
        {
            return await _context.Assignments.Include(a => a.Staff)    
                                             .Include(a => a.Station)  
                                             .FirstOrDefaultAsync(a => a.Id == assignmentId);
        }

        public async Task<Assignment?> GetCurrentAssignmentAsync(string staffId)
        {
            var today = DateTime.Now.Date;

            return await _context.Assignments
                .Include(a => a.Station)
                .Include(a => a.Staff)
                .Where(a => a.StaffId == staffId
                    && a.IsActive == true
                    && a.EffectiveFrom <= today
                    && (a.EffectiveTo == null || a.EffectiveTo >= today)
                )
                .FirstOrDefaultAsync();
        }
    }
}