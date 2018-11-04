using RazorPagesLearning.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using RazorPagesLearning.Service.User;
using RazorPagesLearning.Service.DB;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 在庫サービス
    /// </summary>
    public class StockService : DBServiceBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ref_db"></param>
        /// <param name="ref_user"></param>
        /// <param name="ref_signInManager"></param>
        /// <param name="ref_userManager"></param>
        public StockService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }

        /// <summary>
        /// 書き込み設定
        /// </summary>
        public class WriteConfig
        {
            /// <summary>
            /// 在庫
            /// </summary>
            public STOCK STOCK;
        }

        /// <summary>
        /// 在庫情報の取得
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns> 在庫情報とログイン中ユーザーに関連するユーザー項目 </returns>
        public Tuple<STOCK, USER_ITEM> readWithUserItemByStockId(long stockId)
        {
            var stock = db.STOCKs;

            //在庫情報を読み取る
            var s = stock
                .Where(e => e.ID == stockId)
                .FirstOrDefault();

            //ログイン中のユーザー情報を読み取る
            var u = this.readLoggedUserInfo();
            u.Wait();

            //ユーザー情報を読み取る
            var ui = db.USER_ITEMs.Where(e => e.USER_ACCOUNT_ID == u.Result.ID &&
           e.STOCK_ID == s.ID)
            .FirstOrDefault();

            return Tuple.Create(s, ui);
        }

		/// <summary>
		/// 在庫情報の取得
		/// </summary>
		/// <param name="stockId"></param>
		/// <returns></returns>
        public Result.ExecutionResult<STOCK> readByStockId(long stockId)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<STOCK>((ret) =>
            {
                ret.result = db.STOCKs
                 .Where(e => e.ID == stockId)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }

		/// <summary>
		/// 取得(データ取り込み：その他依頼)
		/// </summary>
		/// <param name="storageNum"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		public Result.ExecutionResult<STOCK> read(string storageNum, string title)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<STOCK>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                ret.result = db.STOCKs
                  .Where(e => e.STORAGE_MANAGE_NUMBER == storageNum)
                  .Where(e => e.TITLE == title)
                  .Where(e => e.SHIPPER_CODE == user.CURRENT_SHIPPER_CODE)
				  .Where(e => e.DELETE_FLAG == false)
				  .Where(e => e.HIDE_FLAG == false)
                  .FirstOrDefault();
            });
        }

		/// <summary>
		/// 取得(再入庫ダイレクト)
		/// </summary>
		/// <param name="barcode"></param>
		/// <returns></returns>
		public Result.ExecutionResult<STOCK> barcodeRead(string barcode)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<STOCK>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                var userDepartment = user.USER_DEPARTMENTs.FirstOrDefault();
                var depCode = userDepartment.DEPARTMENT_CODE;

				// バーコードと所属している部課が一致
				ret.result = db.STOCKs
					.Where(e => e.BARCODE == barcode)
					.Where(e => e.DEPARTMENT_CODE == depCode)
					.Where(e => e.DELETE_FLAG == false)
					.Where(e => e.HIDE_FLAG == false)
					.FirstOrDefault();
            });
        }

        // 追加
        public async Task add(List<STOCK> list)
        {
            await this.setBothManagementInformation(list);
            this.db.STOCKs.AddRange(list);
        }

        // 追加
        public async Task add(STOCK refStock)
        {
            #region ローカル関数

            //ユーザー項目を更新する
            void LF_update_USER_ITEM(STOCK target)
            {
                if (null == target.USER_ITEMs)
                {
                    target.USER_ITEMs = new List<USER_ITEM>();
                }

                //対象を探す
                if (null != refStock.USER_ITEMs)
                {
                    //更新対象一覧を作成する
                    var updates = target.USER_ITEMs
                            .Select(e =>
                            Tuple.Create(e,
                            refStock.USER_ITEMs.Where(in_e =>
                            in_e.STOCK_ID == e.STOCK_ID &&
                            in_e.USER_ACCOUNT_ID == e.USER_ACCOUNT_ID).First()))
                            .ToList();

                    //値の更新を行う
                    foreach (var ele in updates)
                    {
                        AutoMapper.Mapper.Map(ele.Item1, ele.Item2);
                    }

                    //更新対象一覧から、更新済みのデータを消去。
                    //残った値が新規追加となる。
                    {
                        foreach (var ele in updates)
                        {
                            refStock.USER_ITEMs.Remove(ele.Item2);
                        }

                        if (0 != refStock.USER_ITEMs.Count)
                        {
                            target.USER_ITEMs.AddRange(refStock.USER_ITEMs);
                        }
                    }
                }

            }

            #endregion

            var tmp = this.db.STOCKs
                .Include(e => e.USER_ITEMs)
                .FirstOrDefault(e => e.ID == refStock.ID);

            //ユーザー項目を再審の値で更新
            LF_update_USER_ITEM(tmp);

            tmp.PROJECT_NO1 = refStock.PROJECT_NO1;
            tmp.PROJECT_NO2 = refStock.PROJECT_NO2;
            tmp.COPYRIGHT1 = refStock.COPYRIGHT1;
            tmp.COPYRIGHT2 = refStock.COPYRIGHT2;
            tmp.CONTRACT1 = refStock.CONTRACT1;
            tmp.CONTRACT2 = refStock.CONTRACT2;
            tmp.DATA_NO1 = refStock.DATA_NO1;
            tmp.DATA_NO2 = refStock.DATA_NO2;
            tmp.PROCESS_JUDGE1 = refStock.PROCESS_JUDGE1;
            tmp.PROCESS_JUDGE2 = refStock.PROCESS_JUDGE2;

            await this.setUpdateManagementInformation(tmp);
            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// 作業依頼追加の可否判定結果
        /// </summary>
        public class RequestableJudgement
        {
            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage = string.Empty;

            /// <summary>
            /// 在庫
            /// </summary>
            public STOCK stock = new STOCK();
        }

        /// <summary>
        /// 作業依頼追加できるかチェック
        /// </summary>
        /// <param name="mwlStockId"></param>
        /// <param name="userAccountId"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<RequestableJudgement>> isRequestable(Int64 mwlStockId, Int64 userAccountId)
        {
            var stock = readWithUserItemByStockId(mwlStockId).Item1;
            string errorMessage = string.Empty;

            // =====================================================================================
            // 在庫が存在しない
            // =====================================================================================
            if (null == stock)
            {
                errorMessage = "在庫が存在しません。";
            }
            // =====================================================================================
            // 在庫があれば、ステータスをチェックする
            // =====================================================================================
            else
            {
                switch (stock.STATUS)
                {
                    case DOMAIN.StockStatusCode.STOCK:          // 在庫中
                    case DOMAIN.StockStatusCode.SHIPPING:       // 出荷中
                    case DOMAIN.StockStatusCode.REGIST_WAITING: // 登録待
                    case DOMAIN.StockStatusCode.MULTIPLE:       // 複数品
                        var requestListService = new RequestListService(db, user, signInManager, userManager);
                        var requestList = await requestListService.Read(userAccountId, mwlStockId);
                        if (null != requestList.result)
                        {
                            errorMessage = "作業依頼に追加済みです。";
                        }
                        break;

                    case DOMAIN.StockStatusCode.SCRAP:          // 廃棄済
                        errorMessage = "廃棄済みのため、作業依頼追加できません。";
                        break;

                    case DOMAIN.StockStatusCode.PERIPHERY:      // 抹消済
                        errorMessage = "抹消済みのため、作業依頼追加できません。";
                        break;

                    case DOMAIN.StockStatusCode.REQUEST:        // 依頼中
                        errorMessage = "依頼中のため、作業依頼追加できません。";
                        break;

                    default:
                        errorMessage = "ステータスが不明のため、作業依頼追加できません。";
                        break;
                }
            }

            // =====================================================================================
            // エラーがある場合は、作業依頼追加不可
            // =====================================================================================
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return new Result.ExecutionResult<RequestableJudgement>
                {
                    succeed = false,
                    result = new RequestableJudgement
                    {
                        errorMessage = errorMessage,
                        stock = stock
                    }
                };
            }
            // =====================================================================================
            // 作業依頼追加可
            // =====================================================================================
            else
            {
                return new Result.ExecutionResult<RequestableJudgement>
                {
                    succeed = true,
                    result = new RequestableJudgement
                    {
                        stock = stock
                    }
                };
            }
        }

        /// <summary>
        /// 更新
        /// 作業依頼確定(確認、確定)による在庫状態の更新
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public Result.DefaultExecutionResult saveRequestConfirm
            (Int64 targetID, Action<STOCK> updateAction)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagement((ret) =>
          {
              // 対象レコードを探す
              var record = db.STOCKs.Where(e => e.ID == targetID)
            .FirstOrDefault() ?? throw new ArgumentException("指定された在庫がありません。");

              // 値の更新処理を実行
              updateAction(record);

              //時刻更新
              record.UPDATED_AT = DateTimeOffset.Now;

              // DBに登録
              db.STOCKs.Update(record);

              // 実行成功
              ret.succeed = true;
          });
        }
    }
}



