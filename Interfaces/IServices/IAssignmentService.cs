using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Assignment;
using API.Entities;

namespace API.Interfaces.IServices
{
    public interface IAssignmentService
    {
        Task<AssignmentDto> CreateAsync(Assignment assignmentModel);
        Task<AssignmentDto?> GetByIdAsync(int assignmentId);
        Task<AssignmentDto> UpdateAsync(int id, AssignmentUpdateDto updateDto);
    }
}