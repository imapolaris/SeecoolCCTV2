using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTV2InfoDeploy.Models;
using CCTV2InfoDeploy.Persistence;
using CCTV2InfoDeploy.Util;
using CCTVModels.User;

namespace CCTV2InfoDeploy.Controllers
{
    public class UserController : ApiController
    {
        private CCTVUserInfo cloneUser(CCTVUserInfo ui)
        {
            return new CCTVUserInfo()
            {
                UserName = ui.UserName,
                IsAdmin = ui.IsAdmin,
                ChineseName = ui.ChineseName
            };
        }

        //POST api/user/login
        [Route("api/user/login")]
        [HttpPost]
        public IHttpActionResult Login(CCTVUserInfo ui)
        {
            if (ui == null)
                return BadRequest("登录用户信息不能为空");
            if (string.IsNullOrWhiteSpace(ui.UserName))
                return BadRequest("用户名不能为空");
            var user = UserInfoPersistence.Instance.GetInfo(ui.UserName);
            if (user == null)
            {
                if (ui.UserName != "admin")
                    return BadRequest($"用户 {ui.UserName} 不存在");
                else
                {
                    if (ui.Password == "admin")
                    {
                        return Ok(new CCTVUserInfo()
                        {
                            UserName = "admin",
                            IsAdmin = true,
                            ChineseName = "超级管理员"
                        });
                    }
                    else
                    {
                        return BadRequest("密码错误");
                    }
                }
            }
            if (!user.Password.Equals(MD5Encryptor.GetMD5Hash(ui.Password)))
                return BadRequest("密码错误");
            return Ok(cloneUser(user));
        }

        // GET: api/User
        public IEnumerable<UserViewModel> Get()
        {
            IEnumerable<CCTVUserInfo> uis = UserInfoPersistence.Instance.GetAllInfos();
            IEnumerable<CCTVUserPrivilege> uirs = UserPrivilegePersistence.Instance.GetAllInfos();
            Dictionary<string, UserViewModel> dUvms = new Dictionary<string, UserViewModel>();
            foreach (CCTVUserInfo ui in uis)
            {
                UserViewModel uvm = new UserViewModel()
                {
                    User = cloneUser(ui)
                };
                uvm.User.Password = null;
                dUvms[ui.UserName] = uvm;
            }
            foreach (CCTVUserPrivilege ur in uirs)
            {
                if (dUvms.ContainsKey(ur.UserName))
                    dUvms[ur.UserName].Privilege = ur;
            }
            return dUvms.Values.ToArray();
        }

        // GET: api/User/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">即username</param>
        /// <returns></returns>
        public IHttpActionResult Get(string id)
        {
            CCTVUserInfo ui = UserInfoPersistence.Instance.GetInfo(id);
            if (ui != null)
            {
                CCTVUserPrivilege ur = UserPrivilegePersistence.Instance.GetInfo(id);
                UserViewModel uvm = new UserViewModel()
                {
                    User = cloneUser(ui),
                    Privilege = ur
                };
                uvm.User.Password = null;
                return Ok(uvm);
            }
            return BadRequest("无效的用户名");
        }

        // POST: api/User
        public IHttpActionResult Post([FromBody]UserViewModel uvm)
        {
            if (uvm == null || uvm.User == null)
                return BadRequest("数据不能为空");
            if (string.IsNullOrWhiteSpace(uvm.User.UserName) || string.IsNullOrWhiteSpace(uvm.User.Password))
                return BadRequest("用户名或密码不能为空");
            var oldUser = UserInfoPersistence.Instance.GetInfo(uvm.User.UserName);
            if (oldUser != null)
                return BadRequest($"用户 {uvm.User.UserName} 已存在。");
            //MD5加密
            uvm.User.Password = MD5Encryptor.GetMD5Hash(uvm.User.Password);
            UserInfoPersistence.Instance.Put(uvm.User.UserName, uvm.User);
            if (uvm.Privilege != null)
            {
                uvm.Privilege.UserName = uvm.User.UserName;
                UserPrivilegePersistence.Instance.Put(uvm.User.UserName, uvm.Privilege);
            }
            return Ok("添加用户信息成功");
        }

        // PUT: api/User/5
        public IHttpActionResult Put(string id, [FromBody]UserViewModel value)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("用户名不能为空");
            if (value == null || (value.User == null && value.Privilege == null))
                return BadRequest("数据不能为空");
            if (value.User != null)
            {
                value.User.UserName = id;
                if (string.IsNullOrWhiteSpace(value.User.Password))
                {
                    var oldUI = UserInfoPersistence.Instance.GetInfo(id);
                    if (oldUI != null)
                    {
                        value.User.Password = oldUI.Password;
                    }
                }
                else
                {
                    //MD5加密
                    value.User.Password = MD5Encryptor.GetMD5Hash(value.User.Password);
                }
                UserInfoPersistence.Instance.Put(id, value.User);
            }
            if (value.Privilege != null)
            {
                value.Privilege.UserName = id;
                UserPrivilegePersistence.Instance.Put(id, value.Privilege);
            }
            return Ok("修改用户信息成功");
        }

        // DELETE: api/User/5
        public IHttpActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("用户名不能为空");
            if (id.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return BadRequest("不能删除默认管理员用户:admin");
            UserInfoPersistence.Instance.Delete(id);
            UserPrivilegePersistence.Instance.Delete(id);
            return Ok("删除用户信息成功");
        }
    }
}
