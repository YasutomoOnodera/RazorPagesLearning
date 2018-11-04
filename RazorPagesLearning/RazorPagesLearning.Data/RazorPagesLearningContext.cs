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


            #region 複合PrimaryKey

            // 複合主キーはアノテーションでは設定できない
            // https://qiita.com/hahifu/items/58819f6f36433f20884d
            // https://docs.microsoft.com/ja-jp/ef/core/modeling/keys

            // ユーザー項目
            builder.Entity<USER_ITEM>().HasKey(e => new
            {
                e.STOCK_ID,
                e.USER_ACCOUNT_ID
            });

            // ウォッチリスト
            builder.Entity<WATCHLIST>().HasKey(e => new
            {
                e.STOCK_ID,
                e.USER_ACCOUNT_ID
            });

            // 作業依頼一覧
            builder.Entity<REQUEST_LIST>().HasKey(e => new
            {
                e.STOCK_ID,
                e.USER_ACCOUNT_ID
            });

            // 部課マスタ
            builder.Entity<DEPARTMENT_ADMIN>().HasKey(e => new
            {
                e.SHIPPER_CODE,
                e.DEPARTMENT_CODE
            });

            // ユーザー部課
            builder.Entity<USER_DEPARTMENT>().HasKey(e => new
            {
                e.USER_ACCOUNT_ID,
                e.SHIPPER_CODE,
                e.DEPARTMENT_CODE
            });

            // 適応保留中のユーザー部課
            builder.Entity<WK_USER_DEPARTMENT>().HasKey(e => new
            {
                e.USER_ACCOUNT_ID,
                e.SHIPPER_CODE,
                e.DEPARTMENT_CODE
            });

            // ユーザー集配先
            builder.Entity<USER_DELIVERY>().HasKey(e => new
            {
                e.DELIVERY_ADMIN_ID,
                e.USER_ACCOUNT_ID
            });

            // 表示設定
            builder.Entity<USER_DISPLAY_SETTING>().HasKey(e => new
            {
                e.SCREEN_ID,
                e.USER_ACCOUNT_ID,
                e.PHYS_COLUMN_NAME
            });

            // 車両マスタ
            builder.Entity<TRUCK_ADMIN>().HasKey(e => new
            {
                e.TRANSPORT_ADMIN_CODE,
                e.TRUCK_MANAGE_NUMBER
            });

            // 集配依頼
            builder.Entity<DELIVERY_REQUEST>().HasKey(e => new
            {
                e.TRANSPORT_ADMIN_CODE,
                e.DELIVERY_REQUEST_NUMBER
            });

            // ドメイン
            builder.Entity<DOMAIN>().HasKey(e => new
            {
                e.KIND,
                e.CODE
            });

            #endregion // 複合PrimaryKey


            #region 複合PrimaryKeyへのForeignKey

            // リレーション https://docs.microsoft.com/ja-jp/ef/core/modeling/relationships
            // OnDelete https://docs.microsoft.com/ja-jp/ef/core/saving/cascade-delete

            // 集配依頼詳細
            builder.Entity<DELIVERY_REQUEST_DETAIL>()
                .HasOne(drd => drd.TRANSPORT_ADMIN)
                .WithMany(ta => ta.DELIVERY_REQUEST_DETAILs)
                .HasForeignKey(drd => drd.TRANSPORT_ADMIN_CODE)
                .HasPrincipalKey(ta => ta.CODE)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<DELIVERY_REQUEST_DETAIL>()
                .HasOne(drd => drd.DELIVERY_REQUEST)
                .WithMany(dr => dr.DELIVERY_REQUEST_DETAILs)
                .HasForeignKey(drd => new { drd.TRANSPORT_ADMIN_CODE, drd.DELIVERY_REQUEST_NUMBER })
                .HasPrincipalKey(dr => new { dr.TRANSPORT_ADMIN_CODE, dr.DELIVERY_REQUEST_NUMBER });

            // 在庫
            builder.Entity<STOCK>()
                .HasOne(s => s.DEPARTMENT_ADMIN)
                .WithMany(da => da.STOCKs)
                .HasForeignKey(s => new { s.SHIPPER_CODE, s.DEPARTMENT_CODE })
                .HasPrincipalKey(da => new { da.SHIPPER_CODE, da.DEPARTMENT_CODE });

            // ユーザー部課
            builder.Entity<USER_DEPARTMENT>()
                .HasOne(ud => ud.DEPARTMENT_ADMIN)
                .WithMany(da => da.USER_DEPARTMENTs)
                .HasForeignKey(ud => new { ud.SHIPPER_CODE, ud.DEPARTMENT_CODE })
                .HasPrincipalKey(da => new { da.SHIPPER_CODE, da.DEPARTMENT_CODE });

            // ユーザー集配先
            builder.Entity<USER_DELIVERY>()
                .HasOne(dd => dd.USER_ACCOUNT)
                .WithMany(ua => ua.USER_DELIVERies)
                .HasForeignKey(dd => dd.USER_ACCOUNT_ID)
                .HasPrincipalKey(ua => ua.ID);
            builder.Entity<USER_DELIVERY>()
                .HasOne(ud => ud.DELIVERY_ADMIN)
                .WithMany(da => da.USER_DELIVERies)
                .HasForeignKey(ud => ud.DELIVERY_ADMIN_ID)
                .HasPrincipalKey(da => da.ID);

            #endregion // 複合PrimaryKeyへのForeignKey
        }


        #region 在庫系

        /// <summary>
        /// 在庫ワーク
        /// </summary>
        public DbSet<WK_STOCK> WK_STOCKs { get; set; }

        /// <summary>
        /// 在庫
        /// </summary>   
        public DbSet<STOCK> STOCKs { get; set; }

        /// <summary>
        /// ユーザー項目
        /// </summary>   
        public DbSet<USER_ITEM> USER_ITEMs { get; set; }

        #endregion // 在庫系


        #region 依頼系

        /// <summary>
        ///ウォッチリスト
        /// </summary>
        public DbSet<WATCHLIST> WATCHLISTs { get; set; }

        /// <summary>
        /// 作業依頼一覧
        /// </summary>
        public DbSet<REQUEST_LIST> REQUEST_LISTs { get; set; }

        /// <summary>
        /// 作業依頼ワーク
        /// </summary>
        public DbSet<WK_REQUEST> WK_REQUESTs { get; set; }

        /// <summary>
        /// 作業依頼詳細ワーク
        /// </summary>
        public DbSet<WK_REQUEST_DETAIL> WK_REQUEST_DETAILs { get; set; }

        /// <summary>
        /// 集配先ワーク
        /// </summary>
        public DbSet<WK_REQUEST_DELIVERY> WK_REQUEST_DELIVERies { get; set; }

        #endregion // 依頼系


        #region 履歴系

        /// <summary>
        /// 作業依頼履歴
        /// </summary>   
        public DbSet<REQUEST_HISTORY> REQUEST_HISTORies { get; set; }

        /// <summary>
        /// 作業依頼履歴詳細
        /// </summary>   
        public DbSet<REQUEST_HISTORY_DETAIL> REQUEST_HISTORY_DETAILs { get; set; }

        /// <summary>
        /// WMS_作業依頼履歴詳細
        /// </summary>
        public DbSet<WMS_RESULT_HISTORY> WMS_RESULT_HISTORies { get; set; }

        #endregion // 履歴系


        #region 運送会社系

        /// <summary>
        ///運送会社マスタ
        /// </summary>
        public DbSet<TRANSPORT_ADMIN> TRANSPORT_ADMINs { get; set; }

        /// <summary>
        /// 車両マスタ
        /// </summary>
        public DbSet<TRUCK_ADMIN> TRUCK_ADMINs { get; set; }

        /// <summary>
        /// 集配依頼
        /// </summary>
        public DbSet<DELIVERY_REQUEST> DELIVERY_REQUESTs { get; set; }

        /// <summary>
        /// 集配依頼詳細
        /// </summary>
        public DbSet<DELIVERY_REQUEST_DETAIL> DELIVERY_REQUEST_DETAILs { get; set; }

        #endregion // 運送会社系


        #region ユーザー系

        /// <summary>
        /// ユーザーアカウント
        /// </summary>
        public DbSet<USER_ACCOUNT> USER_ACCOUNTs { get; set; }

        /// <summary>
        /// 適応保留中のユーザーアカウント情報
        /// </summary>
        public DbSet<WK_USER_ACCOUNT> WK_USER_ACCOUNTs { get; set; }

        /// <summary>
        /// ユーザー部課
        /// </summary>
        public DbSet<USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

        /// <summary>
        /// 適応保留中のユーザー部課
        /// </summary>
        public DbSet<WK_USER_DEPARTMENT> WK_USER_DEPARTMENTs { get; set; }

        /// <summary>
        /// ユーザー集配先
        /// </summary>
        public DbSet<USER_DELIVERY> USER_DELIVERies { get; set; }

        /// <summary>
        /// 検索条件
        /// </summary>
        public DbSet<USER_SEARCH_CONDITION> USER_SEARCH_CONDITIONs { get; set; }

        /// <summary>
        /// 表示設定
        /// </summary>
        public DbSet<USER_DISPLAY_SETTING> USER_DISPLAY_SETTINGs { get; set; }

        /// <summary>
        /// パスワード履歴
        /// </summary>
        public DbSet<PASSWORD_HISTORY> PASSWORD_HISTORies { get; set; }

        #endregion // ユーザー系


        #region マスタ系

        /// <summary>
        /// 荷主マスタ
        /// </summary>
        public DbSet<SHIPPER_ADMIN> SHIPPER_ADMINs { get; set; }

        /// <summary>
        /// 部課マスタ
        /// </summary>
        public DbSet<DEPARTMENT_ADMIN> DEPARTMENT_ADMINs { get; set; }

        /// <summary>
        /// 集配先マスタ
        /// </summary>  
        public DbSet<DELIVERY_ADMIN> DELIVERY_ADMINs { get; set; }

        /// <summary>
        /// カレンダーマスタ
        /// </summary>
        public DbSet<CALENDAR_ADMIN> CALENDAR_ADMINs { get; set; }

        /// <summary>
        /// メッセージマスタ
        /// </summary> 
        public DbSet<MESSAGE_ADMIN> MESSAGE_ADMINs { get; set; }

        /// <summary>
        ///セキュリティポリシー
        /// </summary>
        public DbSet<POLICY> POLICies { get; set; }

        /// <summary>
        /// ドメイン
        /// </summary>
        public DbSet<DOMAIN> DOMAINs { get; set; }

        /// <summary>
        /// システム設定
        /// </summary>
        public DbSet<SYSTEM_SETTING> SYSTEM_SETTINGs { get; set; }

        /// <summary>
        /// メールテンプレート
        /// </summary>
        public DbSet<MAIL_TEMPLATE> MAIL_TEMPLATEs { get; set; }

        #endregion // マスタ系

        /// <summary>
        /// テーブルにおけるチェックボックス選択状態
        /// </summary>
        public DbSet<WK_TABLE_SELECTION_SETTING> WK_TABLE_SELECTION_SETTINGs { get; set; }

        /// <summary>
        /// テーブルにおけるページネーション設定情報
        /// </summary>
        public DbSet<WK_TABLE_PAGINATION_SETTING> WK_TABLE_PAGINATION_SETTINGs { get; set; }
    }
}
