using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// enum定義のヘルパクラス
    /// </summary>
    public static class USER_ACCOUNTExt
    {
        /// <summary>
        /// 画面に表示する名称の取得
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string DisplayName(this USER_ACCOUNT.ACCOUNT_PERMISSION target)
        {
            string[] names = { "管理者", "作業者", "荷主(編集)", "荷主(閲覧)", "運送会社" };
            return names[(int)(target - 1)];
        }

        /// <summary>
        /// 文字列をenumに変換する
        /// </summary>
        /// <param name="refVal"></param>
        /// <returns></returns>
        public static USER_ACCOUNT.ACCOUNT_PERMISSION toACCOUNT_PERMISSION(this string refVal)
        {
            foreach (USER_ACCOUNT.ACCOUNT_PERMISSION Value in Enum.GetValues(typeof(USER_ACCOUNT.ACCOUNT_PERMISSION)))
            {
                string name = Enum.GetName(typeof(USER_ACCOUNT.ACCOUNT_PERMISSION), Value);
                if (refVal.Trim() == name)
                {
                    return (USER_ACCOUNT.ACCOUNT_PERMISSION)Value;
                }
            }
            throw new ApplicationException("想定外の値です。");
        }
    }

    /// <summary>
    /// ユーザーアカウント
    /// </summary>   
    [Serializable]
    public class USER_ACCOUNT : MODIFY_USER_INFORMATION
    {
        #region 内部使用情報

        /// <summary>
        /// アカウント権限
        /// </summary>
        public enum ACCOUNT_PERMISSION
        {
            /// <summary>
            /// 管理者
            /// </summary>
            Admin = 1,

            /// <summary>
            /// 作業者
            /// </summary>
            Worker = 2,

            /// <summary>
            /// 荷主(編集)
            /// </summary>
            ShipperEditing = 3,

            /// <summary>
            /// 荷主(閲覧)
            /// </summary>
            ShipperBrowsing = 4,

            /// <summary>
            /// 運送会社
            /// </summary>
            ShippingCompany = 5
        }

        #endregion

        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public virtual Int64 ID { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [DisplayName("ユーザーID")]
        public virtual string USER_ID { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        [Required]
        //[StringLength(40)]
        [DisplayName("パスワード")]
        public virtual string PASSWORD { get; set; }

        /// <summary>
        /// パスワードのソルト
        /// </summary>
        public virtual string PASSWORD_SALT { get; set; }

        /// <summary>
        /// 権限
        /// </summary>
		[DisplayName("権限")]
        public virtual ACCOUNT_PERMISSION PERMISSION { get; set; }

        /// <summary>
        /// 選択中荷主コード
        /// </summary>
        [MaxLength(3)]
        [DisplayName("選択中荷主コード")]
        public virtual string CURRENT_SHIPPER_CODE { get; set; }

        /// <summary>
        /// デフォルト部課コード
        /// </summary>
        [StringLength(128)]
        [DisplayName("デフォルト部課コード")]
        public virtual string DEFAULT_DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// 運送会社コード
        /// </summary>
        [StringLength(128)]
        [DisplayName("運送会社コード")]
        public virtual string TRANSPORT_ADMIN_CODE { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(72, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [DisplayName("ユーザー名")]
        public virtual string NAME { get; set; }

        /// <summary>
        /// ユーザー名(カナ)
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [DisplayName("ユーザー名(カナ)")]
        public virtual string KANA { get; set; }

        /// <summary>
        /// 社名
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(72, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [DisplayName("社名")]
        public virtual string COMPANY { get; set; }

        /// <summary>
        /// 部署名
        /// </summary>
        [StringLength(72)]
        [DisplayName("部署名")]
        public virtual string DEPARTMENT { get; set; }

        /// <summary>
        /// 郵便番号
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [RegularExpression(@"\d{7}$", ErrorMessage = "{0}は７桁の数字で入力してください。")]
        [DisplayName("郵便番号")]
        public virtual string ZIPCODE { get; set; }

        /// <summary>
        /// 住所1
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(72, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [DisplayName("住所1")]
        public virtual string ADDRESS1 { get; set; }

        /// <summary>
        /// 住所2
        /// </summary>
        [StringLength(72)]
        [DisplayName("住所2")]
        public virtual string ADDRESS2 { get; set; }

        /// <summary>
        /// TEL
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}は0から14桁の数字で入力してください。")]
        [DisplayName("TEL")]
        public virtual string TEL { get; set; }

        /// <summary>
        /// FAX
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}は0から14桁の数字で入力してください。")]
        [DisplayName("FAX")]
        public virtual string FAX { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("メールアドレス")]
        public virtual string MAIL { get; set; }

        /// <summary>
        /// デフォルト集配先マスタID
        /// </summary>
        [DisplayName("デフォルト集配先マスタID")]
        public virtual Int64 DEFAULT_DELIVERY_ADMIN_ID { get; set; }

        /// <summary>
        /// 同時配信メール1
        /// </summary>
		[StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール1")]
        public virtual string MAIL1 { get; set; }

        /// <summary>
        /// 同時配信メール2
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール2")]
        public virtual string MAIL2 { get; set; }

        /// <summary>
        /// 同時配信メール3
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール3")]
        public virtual string MAIL3 { get; set; }

        /// <summary>
        /// 同時配信メール4
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール4")]
        public virtual string MAIL4 { get; set; }

        /// <summary>
        /// 同時配信メール5
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール5")]
        public virtual string MAIL5 { get; set; }

        /// <summary>
        /// 同時配信メール6
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール6")]
        public virtual string MAIL6 { get; set; }

        /// <summary>
        /// 同時配信メール7
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール7")]
        public virtual string MAIL7 { get; set; }

        /// <summary>
        /// 同時配信メール8
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール8")]
        public virtual string MAIL8 { get; set; }

        /// <summary>
        /// 同時配信メール9
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール9")]
        public virtual string MAIL9 { get; set; }

        /// <summary>
        /// 同時配信メール10
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("同時配信メール10")]
        public virtual string MAIL10 { get; set; }

        /// <summary>
        /// パスワード変更日
        /// </summary>
        [DisplayName("パスワード変更日")]
        public virtual DateTimeOffset? PASSWORD_UPDATED_AT { get; set; }

        /// <summary>
        /// ログイン有効フラグ
        /// </summary>
        [DisplayName("ログイン有効フラグ")]
        public virtual bool LOGIN_ENABLE_FLAG { get; set; }

        /// <summary>
        /// パスワード無期限フラグ
        /// </summary>
        [DisplayName("パスワード無期限フラグ")]
        public virtual bool EXPIRE_FLAG { get; set; }

        /// <summary>
        /// パスワード変更要求
        /// </summary>
        [DisplayName("パスワード変更要求")]
        public virtual bool PASSWORD_CHANGE_REQUEST { get; set; }

        /// <summary>
        /// 確認ダイアログ不要フラグ
        /// </summary>
		[DisplayName("確認ダイアログ不要フラグ")]
        public virtual bool CONFIRM_FLAG { get; set; }

        /// <summary>
        /// 最終ログイン日時
        /// </summary>
        [DisplayName("最終ログイン日時")]
        public virtual DateTimeOffset? LOGINED_AT { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// 運送会社マスタ
        /// </summary>
        [ForeignKey("TRANSPORT_ADMIN_CODE")]
        [DisplayName("運送会社マスタ")]
        public virtual TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

        /// <summary>
        /// ユーザーが所属する部課名
        /// </summary>
        [DisplayName("ユーザー部課")]
        public virtual List<USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

        /// <summary>
        /// 表示設定
        /// </summary>
        [DisplayName("表示設定")]
        public virtual List<USER_DISPLAY_SETTING> DISPLAY_SETTINGs { get; set; }

        /// <summary>
        /// パスワード履歴
        /// </summary>
        [DisplayName("パスワード履歴")]
        public virtual List<PASSWORD_HISTORY> PASSWORD_HISTORYs { get; set; }

        /// <summary>
        /// ユーザー項目
        /// </summary>
        [DisplayName("ユーザー項目")]
        public virtual List<USER_ITEM> USER_ITEMs { get; set; }

        /// <summary>
        /// ウォッチリスト
        /// </summary>
        [DisplayName("ウォッチリスト")]
        public virtual List<WATCHLIST> WATCHLISTs { get; set; }

        /// <summary>
        /// 作業依頼一覧
        /// </summary>
        [DisplayName("作業依頼一覧")]
        public virtual List<REQUEST_LIST> REQUEST_LISTs { get; set; }

        /// <summary>
        /// 作業依頼ワーク
        /// </summary>
        [DisplayName("作業依頼ワーク")]
        public virtual List<WK_REQUEST> WK_REQUESTs { get; set; }

        /// <summary>
        /// ユーザー集配先
        /// </summary>
        [DisplayName("ユーザー集配先")]
        public virtual List<USER_DELIVERY> USER_DELIVERies { get; set; }

        /// <summary>
        /// 適応保留中のユーザーアカウント情報
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
        public virtual WK_USER_ACCOUNT WK_USER_ACCOUNT { get; set; }

        #endregion // Navigation


        /// <summary>
        /// テーブルにおけるチェックボックス選択状態
        /// </summary>
        public virtual List<WK_TABLE_SELECTION_SETTING> WK_TABLE_SELECTION_SETTINGs { get; set; }

        /// <summary>
        /// テーブルにおけるページネーション設定情報
        /// </summary>
        public virtual List<WK_TABLE_PAGINATION_SETTING> WK_TABLE_PAGINATION_SETTINGs { get; set; }

    }
}