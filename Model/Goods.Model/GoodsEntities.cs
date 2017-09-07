namespace Goods.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GoodsEntities : DbContext
    {
        public GoodsEntities()
            : base("name=GoodsEntities")
        {
        }

        public virtual DbSet<Advertisements> Advertisements { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<Userloginrecord> Userloginrecord { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advertisements>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<Advertisements>()
                .Property(e => e.key)
                .IsUnicode(false);

            modelBuilder.Entity<Advertisements>()
                .Property(e => e.img)
                .IsUnicode(false);

            modelBuilder.Entity<Advertisements>()
                .Property(e => e.content)
                .IsUnicode(false);

            modelBuilder.Entity<Advertisements>()
                .Property(e => e.goodskey)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.reason)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.link)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.command)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.price)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.recommender)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.recommendname)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.auditopinion)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.audituser)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.auditname)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.image)
                .IsUnicode(false);

            modelBuilder.Entity<Goods>()
                .Property(e => e.buyimage)
                .IsUnicode(false);

            modelBuilder.Entity<Questions>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<Questions>()
                .Property(e => e.feedbackuserid)
                .IsUnicode(false);

            modelBuilder.Entity<Questions>()
                .Property(e => e.feedbackusernickname)
                .IsUnicode(false);

            modelBuilder.Entity<Questions>()
                .Property(e => e.contact)
                .IsUnicode(false);

            modelBuilder.Entity<Questions>()
                .Property(e => e.content)
                .IsUnicode(false);

            modelBuilder.Entity<Userloginrecord>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<Userloginrecord>()
                .Property(e => e.userid)
                .IsUnicode(false);

            modelBuilder.Entity<Userloginrecord>()
                .Property(e => e.loginaddress)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.id)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.userid)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.nickname)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.idcard)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.realname)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.phonenumber)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.avatar)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.usersignature)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.wechat)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.qq)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.sina)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.taobao)
                .IsUnicode(false);
        }
    }
}
