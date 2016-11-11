using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CCTVModels;
using Persistence;

namespace CCTV2InfoDeploy.Controllers
{
    public class LogicalTreeController : ApiController
    {
        // GET: api/LogicalTree
        public IEnumerable<CCTVLogicalTree> Get()
        {
            List<CCTVLogicalTree> trees = LogicalTreePersistence.Instance.GetAllInfos().ToList();
            if (trees.FirstOrDefault(t => t.LogicalName.Equals(CCTVLogicalTree.DefaultName, StringComparison.OrdinalIgnoreCase)) == null)
            {
                trees.Insert(0, CCTVLogicalTree.GetDefault());
            }
            return trees;
        }

        // GET: api/LogicalTree/5
        [Route("api/LogicalTree/{logicalName}")]
        public CCTVLogicalTree Get(string logicalName)
        {
            CCTVLogicalTree tree = LogicalTreePersistence.Instance.GetInfo(logicalName);
            if (tree == null || CCTVLogicalTree.DefaultName.Equals(logicalName, StringComparison.OrdinalIgnoreCase))
            {
                return CCTVLogicalTree.GetDefault();
            }
            return tree;

        }

        // POST: api/LogicalTree
        public IHttpActionResult Post([FromBody]CCTVLogicalTree tree)
        {
            if (tree == null)
                return BadRequest("逻辑节点树的数据内容为空。");
            if (string.IsNullOrWhiteSpace(tree.LogicalName))
                return BadRequest("必须设置逻辑节点树的逻辑名称。");

            if (string.IsNullOrWhiteSpace(tree.DisplayName))
                tree.DisplayName = tree.LogicalName;
            LogicalTreePersistence.Instance.Put(tree.LogicalName, tree);
            return Ok("添加成功");
        }

        //Put:api/LogicalTree/
        [Route("api/LogicalTree/{logicalname}")]
        public IHttpActionResult Put(string logicalName, [FromBody]CCTVLogicalTree tree)
        {
            if (logicalName == null)
                return BadRequest("无效的节点树逻辑名称");
            if (tree == null)
                return BadRequest("提交的节点树数据是一个空值");
            tree.LogicalName = logicalName;
            LogicalTreePersistence.Instance.Put(logicalName, tree);
            return Ok("修改逻辑节点树成功");
        }

        // DELETE: api/LogicalTree/logical
        [Route("api/LogicalTree/{logicalName}")]
        public IHttpActionResult Delete(string logicalName)
        {
            if (CCTVLogicalTree.DefaultName.Equals(logicalName, StringComparison.OrdinalIgnoreCase))
                return BadRequest("不能删除Default默认节点树");
            LogicalTreePersistence.Instance.Delete(logicalName);

            return Ok("删除成功");
        }
    }
}
