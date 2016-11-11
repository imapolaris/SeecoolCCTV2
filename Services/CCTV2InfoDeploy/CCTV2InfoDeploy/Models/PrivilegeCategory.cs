using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTVModels.User;

namespace CCTV2InfoDeploy.Models
{
    public class PrivilegeCategory
    {
        public static PrivilegeCategory PreDefinedPrivileges
        {
            get
            {
                return createDefaut();
            }
        }

        private static PrivilegeCategory createDefaut()
        {
            List<CCTVPrivilege> rights = new List<CCTVPrivilege>();
            rights.Add(new CCTVPrivilege() { Name = "YingJiZhiHui", DisplayName = "应急指挥", Decription = "应急指挥" });
            rights.Add(new CCTVPrivilege() { Name = "TuiChuWangKong", DisplayName = "退出网控", Decription = "退出网控" });
            rights.Add(new CCTVPrivilege() { Name = "XiuGaiMiMa", DisplayName = "修改密码", Decription = "修改密码" });
            rights.Add(new CCTVPrivilege() { Name = "ShuangXiangDuiJiang", DisplayName = "双向对讲", Decription = "双向对讲" });
            rights.Add(new CCTVPrivilege() { Name = "HongWaiJuJiao", DisplayName = "红外聚焦", Decription = "红外聚焦" });
            rights.Add(new CCTVPrivilege() { Name = "GaoQingSuoFang", DisplayName = "高清缩放", Decription = "高清缩放" });
            return new PrivilegeCategory() { Privileges = rights };
        }

        public List<CCTVPrivilege> Privileges { get; set; }
    }
}
