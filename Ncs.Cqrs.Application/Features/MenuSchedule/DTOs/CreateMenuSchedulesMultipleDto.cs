using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncs.Cqrs.Application.Features.MenuSchedule.DTOs
{
    public class CreateMenuSchedulesMultipleDto
    {
        public List<CreateMenuSchedulesDto> MenuItems { get; set; }
        public string ScheduleDate { get; set; }
    }
}
