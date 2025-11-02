using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Assignment;
using API.Entities;

namespace API.Interfaces.IRepositories
{
    public interface IAssignmentRepository
    {
        Task<Assignment> CreateAsync(Assignment assignmentModel);
        Task<Assignment?> GetByIdAsync(int assignmentId);
        Task<Assignment?> GetCurrentAssignmentAsync(string staffId);
    }
}