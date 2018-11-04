using AutoMapper;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Service.FormattedView
{
    /// <summary>
    /// 画面、レポート表示用に出力形式を自動成形されるビュー
    /// </summary>
    public class Formatted_USER_ACCOUNT : HoldsPendingInformation_USER_ACCOUNT
    {
        //セキュリティポリシー情報を取得する
        private Service.DB.PolicyService policyService;

        public void updateServices(Service.User.UserService ref_userService,
            Service.DB.PolicyService ref_policyService)
        {
            this.userService = ref_userService;
            this.policyService = ref_policyService;
        }

        /// <summary>
        /// フォーマット済みの成形文字列を取得する
        /// </summary>
        /// <returns></returns>
        public string formattedPERMISSION()
        {
            return USER_ACCOUNTExt.DisplayName(this.PERMISSION);
        }

        /// <summary>
        /// 成形済みのデフォルト荷主コード
        /// </summary>
        /// <returns></returns>
        public string formattedCURRENT_SHIPPER_CODE()
        {
            var target = this.CURRENT_SHIPPER_CODE;
            if (null != target)
            {
                if (String.Empty != target)
                {
                    return target;
                }
            }

            return "-";
        }

        /// <summary>
        /// 成形済みの住所
        /// </summary>
        /// <returns></returns>
        public string formattedADDRESS()
        {
            var sb = new System.Text.StringBuilder();
            if (null != this.ADDRESS1)
            {
                sb.Append(this.ADDRESS1);
            }

            if (null != this.ADDRESS2)
            {
                sb.Append(" ");
                sb.Append(this.ADDRESS2);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 成形済みのログイン時間
        /// </summary>
        /// <returns></returns>
        public string formattedLOGINED_AT()
        {
            return RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(this.LOGINED_AT);
        }

        /// <summary>
        /// 成形済みのパスワード更新時間
        /// </summary>
        /// <returns></returns>
        public string formattedPASSWORD_UPDATED_AT()
        {
            return RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(this.PASSWORD_UPDATED_AT);
        }

        /// <summary>
        /// 成形済みのパスワード有効判定
        /// </summary>
        /// <returns></returns>
        public string formattedIsPasswordExpiration()
        {
            //パスワード有効期限を取得
            var interval = policyService.read(new Service.DB.PolicyService.ReadConfig
            {
                NAME = POLICY.PASSWORD_POLICY.Interval
            }).result.VALUE;

            //有効期限に入っているか判定する
            var isIn = userService.isPasswordExpiration(interval, this);

            if (false == isIn)
            {
                return "ログイン可能";
            }
            else
            {
                return "パスワードの有効期間切れ";
            }
        }

        /// <summary>
        /// パスワードが無期限か判定する
        /// </summary>
        /// <returns></returns>
        public string formattedEXPIRE_FLAG()
        {
            if (true == this.EXPIRE_FLAG)
            {
                return "はい";
            }
            else
            {
                return "いいえ";
            }
        }

        /// <summary>
        /// ログイン有効判定
        /// </summary>
        /// <returns></returns>
        public string formattedLOGIN_ENABLE_FLAG()
        {
            if (true == LOGIN_ENABLE_FLAG)
            {
                return "有効";
            }
            else
            {
                return "無効";
            }
        }

        /// <summary>
        /// 成形された運送会社名
        /// </summary>
        /// <returns></returns>
        public string formattedTRANSPORT_ADMIN_NAME()
        {
            string getName()
            {
                if (this.PERMISSION ==
                    Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany)
                {
                    if (null != this.TRANSPORT_ADMIN)
                    {
                        return this.TRANSPORT_ADMIN.NAME;
                    }
                }
                return null;
            }

            var name = getName();
            if (null != name)
            {
                if (String.Empty != name)
                {
                    return name;
                }
            }

            return "-";
        }

        /// <summary>
        /// フォーマットされたユーザー部課
        /// </summary>
        /// <returns></returns>
        public string formattedDEFAULT_DEPARTMENT_CODE()
        {
            if (this.PERMISSION == Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing ||
                this.PERMISSION == Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing)
            {
                if (null != this.DEFAULT_DEPARTMENT_CODE)
                {
                    if (String.Empty != this.DEFAULT_DEPARTMENT_CODE)
                    {
                        return this.DEFAULT_DEPARTMENT_CODE;
                    }
                }
            }

            return "-";
        }

        /// <summary>
        /// 要素を変換する
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Formatted_USER_ACCOUNT convert
            (HoldsPendingInformation_USER_ACCOUNT src,
            Service.User.UserService ref_userService,
            Service.DB.PolicyService ref_policyService)
        {
            var o = Mapper.Map<Formatted_USER_ACCOUNT>(src);
            o.updateServices(ref_userService, ref_policyService);
            return o;
        }
    }

    /// <summary>
    /// 補助関数実装
    /// </summary>
    public static class Formatted_USER_ACCOUNT_Helper
    {
        /// <summary>
        /// カラム列をソートする
        /// </summary>
        /// <param name="src"></param>
        /// <param name="sortDirection"></param>
        /// <param name="targetColumn"></param>
        /// <returns></returns>
        public static IEnumerable<Formatted_USER_ACCOUNT> Sort(
            this IEnumerable<Formatted_USER_ACCOUNT> src,
            SortDirection sortDirection,
            string targetColumn
            )
        {
#region ローカル関数
            //主条件でソートする
            IOrderedEnumerable<Formatted_USER_ACCOUNT> LF_sortByPrimaryKey()
            {
                if (null != targetColumn &&
                    String.Empty != targetColumn)
                {
                    //指定条件に合わせて修正する
                    switch (targetColumn)
                    {
                        //荷主コード
                        case "USER_ACCOUNT_SHIPPER_CODE":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.formattedCURRENT_SHIPPER_CODE());
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.formattedCURRENT_SHIPPER_CODE());
                                        }
                                }
                                break;
                            }
                        //ユーザーID
                        case "USER_ACCOUNT_USER_ID":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.USER_ID);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.USER_ID);
                                        }
                                }
                                break;
                            }
                        //ユーザー名
                        case "USER_ACCOUNT_USER_NAME":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.NAME);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.NAME);
                                        }
                                }
                                break;
                            }
                        //ユーザー名(カナ)
                        case "USER_ACCOUNT_USER_NAME_KANA":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.KANA);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.KANA);
                                        }
                                }
                                break;
                            }
                        //社名
                        case "USER_ACCOUNT_COMPANY":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.COMPANY);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.COMPANY);
                                        }
                                }
                                break;
                            }
                        //部課名
                        case "USER_ACCOUNT_DEPARTMENT":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.DEPARTMENT);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.DEPARTMENT);
                                        }
                                }
                                break;
                            }
                        //住所
                        case "USER_ACCOUNT_ADDRESS":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.formattedADDRESS());
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.formattedADDRESS());
                                        }
                                }
                                break;
                            }
                        //TEL
                        case "USER_ACCOUNT_TEL":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.TEL);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.TEL);
                                        }
                                }
                                break;
                            }
                        //FAX
                        case "USER_ACCOUNT_FAX":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.FAX);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.FAX);
                                        }
                                }
                                break;
                            }
                        //メールアドレス
                        case "USER_ACCOUNT_MAIL":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.MAIL);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.MAIL);
                                        }
                                }
                                break;
                            }
                        //最終ログイン日時
                        case "USER_ACCOUNT_LOGINED_AT":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.LOGINED_AT);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.LOGINED_AT);
                                        }
                                }
                                break;
                            }
                        //最終パスワード変更日時
                        case "USER_ACCOUNT_PASSWORD_UPDATED_AT":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.PASSWORD_UPDATED_AT);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.PASSWORD_UPDATED_AT);
                                        }
                                }
                                break;
                            }
                        //アカウント状態
                        case "USER_ACCOUNT_IS_PASSWORD_EXPIRATION":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.formattedIsPasswordExpiration());
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.formattedIsPasswordExpiration());
                                        }
                                }
                                break;
                            }
                        //パスワード無期限フラグ
                        case "USER_ACCOUNT_EXPIRE_FLAG":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.EXPIRE_FLAG);
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.EXPIRE_FLAG);
                                        }
                                }
                                break;
                            }
                        //ログイン可能判定 修正
                        case "USER_ACCOUNT_LOGIN_ENABLE_FLAG":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.formattedLOGIN_ENABLE_FLAG());
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.formattedLOGIN_ENABLE_FLAG());
                                        }
                                }
                                break;
                            }
                        //運送会社
                        case "USER_ACCOUNT_TRANSPORT":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.formattedTRANSPORT_ADMIN_NAME());
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.formattedTRANSPORT_ADMIN_NAME());
                                        }
                                }
                                break;
                            }
                        //デフォルト部課コード
                        case "USER_ACCOUNT_DEFAULT_DEPARTMENT":
                            {
                                switch (sortDirection)
                                {
                                    case SortDirection.ASC:
                                        {
                                            return src.OrderBy(e => e.formattedDEFAULT_DEPARTMENT_CODE());
                                        }
                                    case SortDirection.DES:
                                        {
                                            return src.OrderByDescending(e => e.formattedDEFAULT_DEPARTMENT_CODE());
                                        }
                                }
                                break;
                            }
                        default:
                            {
                                throw new ApplicationException($"{targetColumn}列をソート条件に指定する事はできません。");
                            }
                    }
                }
                
                //条件がない場合、ユーザーIDを検索条件としておく
                return src.OrderBy(e=>e.USER_ID);
            }
#endregion

            //最終ソート条件とする
            var q = LF_sortByPrimaryKey().ThenBy(e => e.USER_ID);

            return q;
        }
    }
}
