using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Assignment;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.IServices;
using API.Mappers;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;
        public AssignmentService(IUnitOfWork uow, UserManager<AppUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<AssignmentDto> CreateAsync(Assignment assignmentModel)
        {
            var staff = await _userManager.FindByIdAsync(assignmentModel.StaffId);
            var station = await _uow.Stations.GetByIdAsync(assignmentModel.StationId);

            if (staff == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy nhân viên (StaffId: {assignmentModel.StaffId}).");
            }
            if (station == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy trạm sạc (StationId: {assignmentModel.StationId}).");
            }

            List<string> validRoles = new List<string> { AppConstant.Roles.Operator, 
                                                         AppConstant.Roles.Manager, 
                                                         AppConstant.Roles.Technician };

            var userRoles = await _userManager.GetRolesAsync(staff);

            bool hasValidRole = userRoles.Any(role => validRoles.Contains(role));

            if (!hasValidRole)
            {
                string allowedRolesString = string.Join(", ", validRoles);
                throw new InvalidOperationException($"Không thể phân công. Nhân viên phải có một trong các vai trò: {allowedRolesString}.");
            }

            if (assignmentModel.EffectiveTo.HasValue && assignmentModel.EffectiveFrom >= assignmentModel.EffectiveTo)
            {
                throw new InvalidOperationException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
            }

            await _uow.Assignments.CreateAsync(assignmentModel);
            await _uow.Complete();

            assignmentModel.Staff = staff!;
            assignmentModel.Station = station!; 

            return assignmentModel.ToAssignmentDto();
        }

        public async Task<AssignmentDto?> GetAssignmentByStaffIdAsync(string staffId)
        {
            var assignmentModel = await _uow.Assignments.GetCurrentAssignmentAsync(staffId);
            if (assignmentModel == null)
            {
                return null;
            }

            return assignmentModel.ToAssignmentDto();
        }

        public async Task<AssignmentDto?> GetByIdAsync(int assignmentId)
        {
            var assignmentModel = await _uow.Assignments.GetByIdAsync(assignmentId);
            if (assignmentModel == null)
            {
                return null;
            }

            return assignmentModel.ToAssignmentDto();
        }

        public async Task<AssignmentDto> UpdateAsync(int id, AssignmentUpdateDto updateDto)
        {
            var assignment = await _uow.Assignments.GetByIdAsync(id);

            if (assignment == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Phân công với ID: {id}");
            }

            var newEffectiveFrom = updateDto.EffectiveFrom ?? assignment.EffectiveFrom;
            var newEffectiveTo = updateDto.EffectiveTo ?? assignment.EffectiveTo;
            var newStationId = ((updateDto.StationId != assignment.StationId) && (updateDto.StationId != 0)) ? updateDto.StationId : assignment.StationId;
            var newStatus = updateDto.IsActive.HasValue ? updateDto.IsActive.Value : assignment.IsActive;

            if (newEffectiveTo.HasValue && newEffectiveFrom >= newEffectiveTo.Value)
            {
                throw new InvalidOperationException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
            }

            assignment.EffectiveFrom = newEffectiveFrom;
            assignment.EffectiveTo = newEffectiveTo;
            assignment.StationId = newStationId;
            assignment.IsActive = newStatus;
            await _uow.Complete();

            return assignment.ToAssignmentDto();
        }
    }
}