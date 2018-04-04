using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Snow.Orm;
using OrmTest.Models;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrmTest
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult List()
        {
            //List<Models.User> _users = new List<Models.User>();
            //using (Dal dal = Dal.New())
            //{
            //    dal.Gte("id", 1).Find<Models.User>(_users);
            //    return Ok(_users);
            //}
            var now = DateTime.Now;
            var users = Models.User.Table.GetIds(Sql.Condition.Gte(Models.User.Field.Id, 50000).Page().Desc("id"));
            var list = new List<Models.User>(users.Length);
            foreach (var id in users)
            {
                list.Add(Models.User.Table.Get(id));
            }
            Console.WriteLine(DateTime.Now.Subtract(now).TotalMilliseconds);
            var _list = Models.User.Table.GetJson(list);
            Console.WriteLine(DateTime.Now.Subtract(now).TotalMilliseconds);
            return Ok(_list);
        }
        [HttpGet("update/{id}/{nick}")]
        public IActionResult Update(long id, string nick)
        {
            var ok = Models.User.Table.Update(Sql.Condition.ID(id).Set(Models.User.Field.Nick, nick));
            return Ok(ok);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("page")]
        public IActionResult Page()
        {
            List<Models.User> _users = new List<Models.User>();
            using (Dal dal = Dal.New())
            {
                dal.Gte("id", 1).Page(0, 10).Find<Models.User>(_users);
                return Ok(_users);
            }
        }
        /// <summary>
        /// 查询前几条数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("top")]
        public IActionResult Top()
        {
            List<Models.User> _users = new List<Models.User>();
            using (Dal dal = Dal.New())
            {
                dal.Gte("id", 1).Top(10).Find<Models.User>(_users);
                return Ok(_users);
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns>表</returns>
        [HttpGet]
        public IActionResult Query()
        {
            using (Dal dal = Dal.New())
            {
                var dt = dal.Query("select * from user limit 20");
                return Ok(dt);
            }
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //using (var dal = Dal.New())
            //{
            //    var user = new Models.User();
            //    user.Id = id;
            //    dal.Get(user);
            //    return Ok(user);
            //}

            var user = Models.User.Table.Get(id);
            return Ok(Models.User.Table.GetJson(user));
        }
        [HttpGet("gets/{index}")]
        public IActionResult Gets(uint index)
        {
            var _bean = new Models.User();
            _bean.BirthDay = 19720101;
            //var users = Models.User.Table.GetIds(_bean,null,50) ;
            var users = Models.User.Table.GetIds(Sql.Condition.Eq(Models.User.Field.BirthDay, 19720101).Page(index).Desc("id"));
            return Ok(users);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
