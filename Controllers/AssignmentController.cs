using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Assignment;
using API.Interfaces;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Mappers;
using API.Helpers;

namespace API.Controllers
{
    [Route("api/assignments")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;
        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAssignmentById([FromRoute] int id)
        {
            try
            {
                var assignment = await _assignmentService.GetByIdAsync(id);
                return Ok(assignment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("staff/{staffId}")]
        public async Task<IActionResult> GetAssignmentByStaffId([FromRoute] string staffId)
        {
            try
            {
                var assignment = await _assignmentService.GetAssignmentByStaffIdAsync(staffId);
                return Ok(assignment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newAssignmentDto = await _assignmentService.CreateAsync(createDto.ToAssignmentFromCreateDto());
                return CreatedAtAction(nameof(GetAssignmentById), new { id = newAssignmentDto.Id }, newAssignmentDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateAssignment([FromRoute] int id,
                                                    [FromBody] AssignmentUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedAssignmentDto = await _assignmentService.UpdateAsync(id, updateDto);

                return Ok(updatedAssignmentDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}