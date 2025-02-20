using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Commands;
using Ncs.Cqrs.Application.Features.Menu.DTOs;
using Ncs.Cqrs.Application.Features.Menu.Queries;
using Ncs.Cqrs.Application.Features.MenuSchedule.Commands;
using Ncs.Cqrs.Application.Features.MenuSchedule.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Cqrs.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/menu-items")]
    [ApiVersion("1.0")]
    public class MenuController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMediator mediator, IHttpContextAccessor httpContextAccessor, ILogger<MenuController> logger)
            : base(httpContextAccessor, logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        private async Task<string?> SaveFileAsync(IFormFile file)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                return $"{baseUrl}/uploads/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file.");
                return null;
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
                    _logger.LogInformation("Deleted file: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file.");
            }
        }

        #region Menu Management

        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<MenuItemsDto>>>> GetMenuItems()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetMenuItemsAllQuery()),
                "Error fetching menu items"
            );

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<MenuItemsDto>>> GetMenuItemsById(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetMenuItemsByIdQuery { Id = id }),
                $"Error fetching menu item with ID {id}"
            );

        [HttpPost]
        public async Task<ActionResult<ResponseDto<bool>>> CreateMenuItems([FromForm] CreateMenuItemsDto data)
        {
            if (data.Image == null || data.Image.Length == 0)
                return BadRequest("Image file is required.");

            var imageUrl = await SaveFileAsync(data.Image);
            if (string.IsNullOrEmpty(imageUrl))
                return BadRequest("Failed to save the image file.");

            return await HandleRequestAsync(
                async () =>
                {
                    var result = await _mediator.Send(new CreateMenuItemsCommand
                    {
                        VendorId = data.VendorId,
                        Name = data.Name,
                        Description = data.Description,
                        Calories = data.Calories,
                        Price = data.Price,
                        ImageUrl = imageUrl
                    });

                    if (!result.Success)
                        DeleteFile(imageUrl);

                    return result;
                },
                "Error creating menu item"
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateMenuItems(int id, [FromForm] UpdateMenuItemsDto data)
        {
            string oldImageUrl = string.Empty;
            string newImageUrl = string.Empty;

            var existingMenuItem = await _mediator.Send(new GetMenuItemsByIdQuery { Id = id });
            if (existingMenuItem == null || !existingMenuItem.Success || existingMenuItem.Data == null)
                return NotFound("Menu item not found.");

            oldImageUrl = existingMenuItem.Data.ImageUrl;

            if (data.Image != null && data.Image.Length > 0)
            {
                newImageUrl = await SaveFileAsync(data.Image) ?? string.Empty;
                if (string.IsNullOrEmpty(newImageUrl))
                    return BadRequest("Failed to save the new image file.");
            }

            return await HandleRequestAsync(
                async () =>
                {
                    var result = await _mediator.Send(new UpdateMenuItemsCommand
                    {
                        Id = id,
                        Name = data.Name,
                        Description = data.Description,
                        Calories = data.Calories,
                        Price = data.Price,
                        ImageUrl = !string.IsNullOrEmpty(newImageUrl) ? newImageUrl : oldImageUrl
                    });

                    if (!result.Success && !string.IsNullOrEmpty(newImageUrl))
                        DeleteFile(newImageUrl);
                    else if (!string.IsNullOrEmpty(newImageUrl) && !string.IsNullOrEmpty(oldImageUrl))
                        DeleteFile(oldImageUrl);

                    return result;
                },
                $"Error updating menu item with ID {id}"
            );
        }

        [HttpPut("activate/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> ActivateMenu(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeMenuItemsStatusCommand { Id = id, IsActive = true }),
                $"Error activating menu item with ID {id}"
            );

        [HttpPut("deactivate/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeactivateMenu(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeMenuItemsStatusCommand { Id = id, IsActive = false }),
                $"Error deactivating menu item with ID {id}"
            );

        #endregion

        #region Menu Schedule

        [HttpGet("schedule/{id}")]
        public async Task<ActionResult<ResponseDto<MenuSchedulesDto>>> GetMenuScheduleById(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetMenuSchedulesByIdQuery { Id = id }),
                $"Error fetching menu schedule with ID {id}"
            );

        [HttpPost("schedule")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateMenuSchedule([FromBody] CreateMenuSchedulesDto data)
            => await HandleRequestAsync(
                async () =>
                {
                    var menus = new List<MenuItemsAvailableQuantityDto>
                    {
                        new() { MenuItemsId = data.MenuItemsId1st, AvailableQuantity = data.AvailableQuantity1st },
                        new() { MenuItemsId = data.MenuItemsId2nd, AvailableQuantity = data.AvailableQuantity2nd }
                    };

                    return await _mediator.Send(new CreateMenuSchedulesCommand
                    {
                        MenuItems = menus,
                        ScheduleDate = data.ScheduleDate,
                        IsWeekly = data.IsWeekly,
                        IsMonthly = data.IsMonthly
                    });
                },
                "Error creating menu schedule"
            );

        #endregion
    }
}
