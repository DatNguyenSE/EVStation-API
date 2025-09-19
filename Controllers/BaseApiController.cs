
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")] 
public class BaseApiController : ControllerBase
{

}

// cấu hình chung cho các Controller -> AccountController -> account (không lấy chữ Controller)