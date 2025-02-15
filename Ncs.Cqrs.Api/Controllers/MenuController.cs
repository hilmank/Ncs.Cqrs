using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Features.Menu.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Queries;
using Ncs.Cqrs.Application.Features.MenuSchedule.Commands;
using Ncs.Cqrs.Application.Features.MenuSchedule.DTOs;
using Ncs.Cqrs.Application.Features.MenuSchedule.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/menu-items")]
    public class MenuController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MenuController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }
        private async Task<string?> SaveFileAsync(IFormFile file)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                return $"{baseUrl}/uploads/{uniqueFileName}"; // ✅ Return full URL
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
                return null; // ✅ Return null if something goes wrong
            }
        }

        private void DeleteFile(string fileUrl)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", Path.GetFileName(new Uri(fileUrl).LocalPath));

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    Console.WriteLine($"Deleted file: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
            }
        }

        #region Modul Menu Management
        /// <summary> Retrieve all MenuItems. </summary>
        [HttpGet()]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuItemsDto>>>> GetMenuItems()
        {
            var result = await _mediator.Send(new GetMenuItemsAllQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve MenuItems by ID. </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<MenuItemsDto>>> GetMenuItemsById(int id)
        {
            var result = await _mediator.Send(new GetMenuItemsByIdQuery { Id = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Add a new MenuItems. </summary>
        [HttpPost()]
        public async Task<ActionResult<ResponseDto<bool>>> CreateMenuItems([FromForm] CreateMenuItemsDto data)
        {
            // Ensure the file is not null
            if (data.Image == null || data.Image.Length == 0)
            {
                return BadRequest("Image file is required.");
            }

            // Try to save the image file
            var imageUrl = await SaveFileAsync(data.Image);

            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Failed to save the image file.");
            }

            // Call MediatR command
            var result = await _mediator.Send(new CreateMenuItemsCommand
            {
                VendorId = data.VendorId,
                Name = data.Name,
                Description = data.Description,
                Calories = data.Calories,
                Price = data.Price,
                ImageUrl = imageUrl
            });

            // If the command fails, delete the saved file
            if (!result.Success)
            {
                DeleteFile(imageUrl);
                return HandleErrorResponse(result);
            }

            return Ok(result);
        }


        /// <summary> Update a MenuItems. </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateMenuItems(int id, [FromForm] UpdateMenuItemsDto data)
        {
            string newImageUrl = string.Empty;
            string oldImageUrl = string.Empty;

            // Fetch existing menu item to get the old image URL
            var existingMenuItem = await _mediator.Send(new GetMenuItemsByIdQuery { Id = id });
            if (existingMenuItem == null || !existingMenuItem.Success || existingMenuItem.Data == null)
            {
                return NotFound("Menu item not found.");
            }

            oldImageUrl = existingMenuItem.Data.ImageUrl; // Store old image URL

            if (data.Image != null && data.Image.Length > 0)
            {
                // Save the new image file
                newImageUrl = await SaveFileAsync(data.Image) ?? string.Empty;

                if (string.IsNullOrEmpty(newImageUrl))
                {
                    return BadRequest("Failed to save the new image file.");
                }
            }

            // Send update command
            var result = await _mediator.Send(new UpdateMenuItemsCommand
            {
                Id = id,
                Name = data.Name,
                Description = data.Description,
                Calories = data.Calories,
                Price = data.Price,
                ImageUrl = !string.IsNullOrEmpty(newImageUrl) ? newImageUrl : oldImageUrl // Use new image if uploaded
            });

            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(newImageUrl))
                {
                    DeleteFile(newImageUrl); // Delete newly saved image if update fails
                }
                return HandleErrorResponse(result);
            }

            // If update successful, delete the old image
            if (!string.IsNullOrEmpty(newImageUrl) && !string.IsNullOrEmpty(oldImageUrl))
            {
                DeleteFile(oldImageUrl);
            }

            return Ok(result);
        }
        /// <summary> Deactivate a menu item. </summary>
        [HttpPut("activate/{id}")]
        [SwaggerOperation(Summary = "Activate a menu item", Description = "Sets the menu item's status to active.")]
        public async Task<ActionResult<ResponseDto<bool>>> ActivateMenu(int id)
        {
            var command = new ChangeMenuItemsStatusCommand { Id = id, IsActive = true };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Deactivate a menu item. </summary>
        [HttpPut("deactivate/{id}")]
        [SwaggerOperation(Summary = "Deactivate a menu item", Description = "Sets the menu item's status to inactive.")]
        public async Task<ActionResult<ResponseDto<bool>>> DeactivateUser(int id)
        {
            var command = new ChangeMenuItemsStatusCommand { Id = id, IsActive = false };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        #endregion
        #region Modul Menu Schedule
        [HttpGet("schedule/{id}")]
        public async Task<ActionResult<ResponseDto<MenuSchedulesDto>>> GetMenuScheduleById(int id)
        {
            var result = await _mediator.Send(new GetMenuSchedulesByIdQuery { Id = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpGet("schedule/weekly")]
        [SwaggerOperation(
            Summary = "Get weekly menu schedules",
            Description = "Retrieves all menu schedules for the given week starting from the specified date.\nThe starting date (startDate) of the week in YYYY-MM-DD format."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuSchedulesDto>>>> GetWeeklyMenuSchedules(string startDate)
        {
            if (!DateTime.TryParse(startDate, out var parsedStartDate))
            {
                return BadRequest("Invalid date format. Please provide a valid date (YYYY-MM-DD).");
            }
            var result = await _mediator.Send(new GetWeeklyMenuSchedulesQuery
            {
                StartDate = parsedStartDate
            });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        [HttpGet("schedule/daily")]
        [SwaggerOperation(
            Summary = "Get daily menu schedules",
            Description = "Retrieves all menu schedules for the specified date.\nThe starting date (date) of the week in YYYY-MM-DD format."
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuSchedulesDto>>>> GetDailyMenuSchedules(string date)
        {
            if (!DateTime.TryParse(date, out var parsedStartDate))
            {
                return BadRequest("Invalid date format. Please provide a valid date (YYYY-MM-DD).");
            }
            var result = await _mediator.Send(new GetDailyMenuSchedulesQuery
            {
                Date = parsedStartDate
            });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        [HttpGet("schedule/monthly")]
        [SwaggerOperation(
            Summary = "Get monthly menu schedules",
            Description = "Retrieves all menu schedules for the specified month and year.\nThe month for which menu schedules are retrieved (1-12).\nThe year for which menu schedules are retrieved"
        )]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuSchedulesDto>>>> GetMonthlyMenuSchedules(int month, int year)
        {
            var result = await _mediator.Send(new GetMonthlyMenuSchedulesQuery { Month = month, Year = year });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }


        /// <summary> Add a new Menu Schedules. </summary>
        [HttpPost("schedule")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateMenuSchedule([FromBody] CreateMenuSchedulesDto data)
        {
            List<MenuItemsAvailableQuantityDto> menus =
            [
                new MenuItemsAvailableQuantityDto{ MenuItemsId = data.MenuItemsId1st,AvailableQuantity = data.AvailableQuantity1st},
                new MenuItemsAvailableQuantityDto{ MenuItemsId = data.MenuItemsId2nd,AvailableQuantity = data.AvailableQuantity2nd}
            ];
            var result = await _mediator.Send(new CreateMenuSchedulesCommand
            {
                MenuItems = menus,
                ScheduleDate = data.ScheduleDate,
                IsWeekly = data.IsWeekly,
                IsMonthly = data.IsMonthly
            });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Update a Menu Schedules. </summary>
        [HttpPut("schedule/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateMenuSchedule(int id, [FromBody] UpdateMenuSchedulesDto data)
        {
            var result = await _mediator.Send(new UpdateMenuSchedulesCommand
            {
                Id = id,
                AvailableQuantity = data.AvailableQuantity
            });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Delete a Menu Schedules. </summary>
        [HttpDelete("schedule/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteMenuSchedule(int id)
        {
            var result = await _mediator.Send(new DeleteMenuSchedulesCommand { Id = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        #endregion
    }
}
