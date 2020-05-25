using System.Collections.Generic;
using System.Linq;
using Calabonga.DemoClasses;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.Swagger.Controllers
{
    [Route("[controller]")]
    public class ApiController: ControllerBase
    {
        private readonly List<Person> _people = People.GetPeople();

        [Route("[action]")]
        public IActionResult GetAll()
        {
            return Ok(_people);
        }

        [Route("[action]/{id:int}")]
        public IActionResult GetById(int id)
        {
            var item = _people.FirstOrDefault(x => x.Id == id);
            return Ok(item);
        }
    }
}
