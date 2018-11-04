using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// テーブル設定サービス
    /// </summary>
    public class TableSelectionSettingService : UpdatableDBServiceBase
    {

        public TableSelectionSettingService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }


        /// <summary>
        /// 追跡用識別子情報
        /// </summary>
        public class TrackingIdentifier
        {
            /// <summary>
            /// 対象となるデータのID
            /// </summary>
            public Int64 dataId { get; set; }

        }

        /// <summary>
        /// 情報履歴テーブルがなかったら追加する
        /// </summary>
        /// <param name="user"></param>
        /// <param name="USER_ID"></param>
        /// <param name="trackingIdentifier"></param>
        /// <param name="viewTableType"></param>
        /// <returns></returns>
        public WK_TABLE_SELECTION_SETTING addNew(
            USER_ACCOUNT user,
            TrackingIdentifier trackingIdentifier,
            ViewTableType viewTableType)
        {
            var t = new Data.Models.WK_TABLE_SELECTION_SETTING
            {
                //Memo
                //複合キーが必要になったらここを変更する
                originalDataId = trackingIdentifier.dataId,
                viewTableType = viewTableType,
                USER_ACCOUNT_ID = user.ID
            };

            if (null == user.WK_TABLE_SELECTION_SETTINGs)
            {
                user.WK_TABLE_SELECTION_SETTINGs = new List<WK_TABLE_SELECTION_SETTING>();
            }

            user.WK_TABLE_SELECTION_SETTINGs.Add(t);
            return t;
        }

        /// <summary>
        /// 読み取り設定
        /// </summary>
        public class ReadConfig
        {
            /// <summary>
            /// ユーザーアカウントID
            /// </summary>
            public long USER_ACCOUNT_ID { get; set; }

            /// <summary>
            /// 対象テーブル種別
            /// </summary>
            public ViewTableType viewTableType { get; set; }

            /// <summary>
            /// 関連DB追跡用の識別子
            /// </summary>
            public TrackingIdentifier trackingIdentifier { get; set; }

        }

        /// <summary>
        /// 指定された情報を読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public WK_TABLE_SELECTION_SETTING read(ReadConfig readConfig)
        {
            var q = db.WK_TABLE_SELECTION_SETTINGs
                .Where(e => 
                (e.viewTableType == readConfig.viewTableType) &&
                (e.USER_ACCOUNT_ID == readConfig.USER_ACCOUNT_ID)).AsQueryable();

            //追跡IDが増えた場合、追跡ID毎に検索条件を張る
            q = q.Where(e => (e.originalDataId == readConfig.trackingIdentifier.dataId));

            return q.FirstOrDefault();
        }

        /// <summary>
        /// 読み取り設定
        /// </summary>
        public class ReadExcludedConfig
        {
            /// <summary>
            /// ユーザーアカウントID
            /// </summary>
            public long USER_ACCOUNT_ID { get; set; }

            /// <summary>
            /// 対象テーブル種別
            /// </summary>
            public ViewTableType viewTableType { get; set; }

            /// <summary>
            /// 除外対象となるデータ対象
            /// </summary>
            public IEnumerable<TrackingIdentifier> excludedIds { get; set; }

        }

        /// <summary>
        /// 指定されたIDを含まない情報を読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<WK_TABLE_SELECTION_SETTING> readExcludedId(ReadExcludedConfig readConfig)
        {
            var q = db.WK_TABLE_SELECTION_SETTINGs
                .Where(e=>(e.viewTableType == readConfig.viewTableType) &&
                (e.USER_ACCOUNT_ID == readConfig.USER_ACCOUNT_ID));

            //Memo
            //複合キーが必要となった場合、以下に複合キーの検索条件を追加する
            q = q.Where(e => (false == readConfig.excludedIds.Select(ine=>ine.dataId).Contains(e.originalDataId)));

            return q;
        }

        /// <summary>
        /// 更新設定
        /// </summary>
        public class UpdateConfig
        {
            /// <summary>
            /// ユーザーアカウントID
            /// </summary>
            public long USER_ACCOUNT_ID { get; set; }

            /// <summary>
            /// 対象テーブル種別
            /// </summary>
            public ViewTableType viewTableType { get; set; }

            /// <summary>
            /// 選択状況
            /// </summary>
            public bool selected { get; set; }

        }

        public void setAllIsSelectedState(UpdateConfig config)
        {
            foreach (var ele in db.WK_TABLE_SELECTION_SETTINGs
                .Where(e => (e.USER_ACCOUNT_ID == config.USER_ACCOUNT_ID) &&
                (e.viewTableType == config.viewTableType)))
            {
                ele.selected = config.selected;
            }
        }

    }
}
