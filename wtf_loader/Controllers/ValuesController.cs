using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace wtf_loader.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IHelper _helper;

        public ValuesController(IHelper helper)
        {
            _helper = helper;
        }
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    public interface IHelper
    {
    }

    class Helper : IHelper
    {
    }
}
