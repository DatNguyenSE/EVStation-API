using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Pricing;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Controllers
{
    [ApiController]
    [Route("api/pricing")]
    [Authorize(Roles = AppConstant.Roles.Admin)]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;
        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        /// <summary>
        /// Lấy tất cả các cấu hình giá
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPricings()
        {
            var pricings = await _pricingService.GetAllPricingsAsync();
            return Ok(pricings);
        }

        /// <summary>
        /// Lấy một cấu hình giá theo ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPricingById([FromRoute] int id)
        {
            try
            {
                var pricing = await _pricingService.GetPricingByIdAsync(id);
                return Ok(pricing);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lấy cấu hình giá đang hoạt động (active) theo loại giá
        /// </summary>
        [HttpGet("active/{priceType}")]
        public async Task<IActionResult> GetCurrentActivePriceByType([FromRoute] PriceType priceType)
        {
            try
            {
                var pricing = await _pricingService.GetCurrentActivePriceByTypeAsync(priceType);
                return Ok(pricing);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Tạo một cấu hình giá mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePricing([FromBody] CreatePricingDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newPricing = await _pricingService.CreatePricingAsync(createDto);
                // Trả về 201 Created cùng với location của resource mới
                return CreatedAtAction(nameof(GetPricingById), new { id = newPricing.Id }, newPricing);
            }
            catch (InvalidOperationException ex)
            {
                // Lỗi nghiệp vụ (ví dụ: trùng ngày, sai logic)
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật một cấu hình giá
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePricing(int id, [FromBody] UpdatePricingDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _pricingService.UpdatePricingAsync(id, updateDto);
                return NoContent(); // 204 No Content là chuẩn cho PUT/UPDATE thành công
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Lỗi nghiệp vụ (ví dụ: trùng ngày, sai logic)
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Vô hiệu hóa (xóa mềm) một cấu hình giá
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeactivatePricing(int id)
        {
            try
            {
                await _pricingService.DeactivatePricingAsync(id);
                return NoContent(); // 204 No Content là chuẩn cho DELETE thành công
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}