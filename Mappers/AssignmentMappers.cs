using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Assignment;
using API.Entities;

namespace API.Mappers
{
    public static class AssignmentMappers
    {
        public static AssignmentDto ToAssignmentDto(this Assignment assignmentModel)
        {
            return new AssignmentDto
            {
                Id = assignmentModel.Id,
                EffectiveFrom = assignmentModel.EffectiveFrom,
                EffectiveTo = assignmentModel.EffectiveTo,
                IsActive = assignmentModel.IsActive,
                Staff = assignmentModel.Staff.ToStaffDto(),
                Station = assignmentModel.Station.ToStationDto()
            };
        }

        public static Assignment ToAssignmentFromCreateDto(this AssignmentCreateDto assignmentDto)
        {
            return new Assignment
            {
                EffectiveFrom = assignmentDto.EffectiveFrom,
                EffectiveTo = assignmentDto.EffectiveTo, 
                StaffId = assignmentDto.StaffId,
                StationId = assignmentDto.StationId,
                IsActive = true 
            };
        }
    }
}