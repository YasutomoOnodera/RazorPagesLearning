using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesLearning.Data
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// DBコンテキスト
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public class RazorPagesLearningContext : IdentityDbContext
    {
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="options"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		public RazorPagesLearningContext(DbContextOptions<RazorPagesLearningContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// モデル生成
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

		#region ユーザー系

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		public DbSet<USER_ACCOUNT> USER_ACCOUNTs { get; set; }

		#endregion // ユーザー系
    }
}
