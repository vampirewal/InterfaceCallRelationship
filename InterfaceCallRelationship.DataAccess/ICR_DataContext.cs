#region << 文 件 说 明 >>
/*----------------------------------------------------------------
// 文件名称：ICR_DataContext
// 创 建 者：杨程
// 创建时间：2021/12/10 9:35:46
// 文件版本：V1.0.0
// ===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/
#endregion

using InterfaceCallRelationship.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vampirewal.Core.DBContexts;
using Vampirewal.Core.Interface;

namespace InterfaceCallRelationship.DataAccess
{
    public class ICR_DataContext: CustomDbContextBase
    {
        public ICR_DataContext(IAppConfig config) : base(config)
        {
            //构造函数
        }

        public DbSet<SystemClass> systems { get; set; }
        public DbSet<ModuleClass> Modules { get; set; }
        public DbSet<FunctionClass> Functions { get; set; }
        public DbSet<MethodClass> Methods { get; set; }

        public DbSet<MethodClassReferenceRelationship> ReferenceRelationships { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(AppConfig.ConnectionStrings[0].Value))
            {
                base.OnConfiguring(optionsBuilder);
                //optionsBuilder.UseSqlite("Data Source=VCodeGeneratorDataBase.db");
                optionsBuilder.UseSqlite(AppConfig.ConnectionStrings[0].Value);
            }
        }

        protected override void InitData()
        {
            if (!string.IsNullOrEmpty(AppConfig.ConnectionStrings[0].Value))
            {
                this.Database.EnsureCreated();
            }
        }

        
    }
}
