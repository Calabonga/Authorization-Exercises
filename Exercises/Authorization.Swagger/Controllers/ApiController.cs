using System.Collections.Generic;
using System.Linq;
using Calabonga.DemoClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Swagger.Controllers
{
    [Route("[controller]")]
    public class ApiController: ControllerBase
    {
        private readonly List<Person> _people = People.GetPeople();

        [HttpGet("[action]")]
        [Authorize]
        public IActionResult GetAll()
        {
            return Ok(_people);
        }

        [HttpGet("[action]/{id:int}")]
        public IActionResult GetById(int id)
        {
            var item = _people.FirstOrDefault(x => x.Id == id);
            return Ok(item);
        }
    }
}
