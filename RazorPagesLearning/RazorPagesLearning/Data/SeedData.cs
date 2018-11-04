using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Data
{
    public static class SeedData
    {

        /// <summary>
        /// 処理権限を初期化する
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task<bool> initUserRoles(IServiceProvider serviceProvider)
        {
            Func<string, RoleManager<IdentityRole>, Task<bool>> make = async (name, roleManager) =>
             {
                 var roleCheck = await roleManager.RoleExistsAsync(name);
                 if (!roleCheck)
                 {
                     //create the roles and seed them to the database
                     var roleResult = await roleManager.CreateAsync(new IdentityRole(name));
                     if (false == roleResult.Succeeded)
                     {
                         throw new ApplicationException($"ユーザー権限(name)の追加に失敗しました。");
                     }
                 }
                 return true;
             };

            //権限を生成する
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //管理者
            await make(USER_ACCOUNT.ACCOUNT_PERMISSION.Admin.ToString(), RoleManager);
            //荷主　閲覧
            await make(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing.ToString(), RoleManager);
            //荷主 編集
            await make(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing.ToString(), RoleManager);
            //運送会社
            await make(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany.ToString(), RoleManager);
            //作業者
            await make(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.Worker.ToString(), RoleManager);

            return true;

        }
        /// <summary>
        /// 管理者ユーザーを追加する
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static async Task<bool> initAdminUser(IServiceProvider serviceProvider)
        {
            var adminId = 0;
            var adminName = "MWLSystemAdmin";
            var adminPass = "MWLSystemAdmin2018";

            //認証機構を取得する
            var user = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var userService = new RazorPagesLearning.Service.User.UserService
                (serviceProvider.GetRequiredService<RazorPagesLearningContext>(), null, null,
               user);

            var f = userService.read(adminName);
            if (null == f)
            {
                //サービスを使ってユーザーを追加する
                var r = await userService.add(
                new Service.User.UserService.ChangeConfig
                {
                    password = adminPass,
                    USER_ACCOUNT = new USER_ACCOUNT
                    {
                        USER_ID = adminName,
                        NAME = adminName,
                        KANA = adminName,
                        COMPANY = "三井物産グローバルロジスティクス株式会社",
                        ADDRESS1 = "東京都港区東新橋2-14-1",
                        ZIPCODE = "1050021",
                        TEL = "0356571130",
                        FAX = "0356571130",
                        MAIL = "hoge@test.com",
                        PERMISSION = USER_ACCOUNT.ACCOUNT_PERMISSION.Admin,
                        LOGIN_ENABLE_FLAG = true
                    },
                    createdUserAccountId = adminId,
                    updatedUserAccountId = adminId
                });
                if (false == r.succeed)
                {
                    //ここでエラーが出ていたらどうにもならないので例外投げて終了とする。
                    throw new ApplicationException(r.errorMessages.FirstOrDefault());
                }
            }

            return true;
        }

        /// <summary>
        /// セキュリティポリシーを追加する
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static void initPOLICY(IServiceProvider serviceProvider)
        {
            //登録用データ
            var vals = new List<POLICY> {
                //パスワード桁数
                new POLICY{
                    NAME = POLICY.PASSWORD_POLICY.Digit,
                    VALUE = 8
                },
                //パスワード更新間隔
                new POLICY{
                    NAME = POLICY.PASSWORD_POLICY.Interval,
                    VALUE = 90
                },
                //パスワード更新通知期間
                new POLICY{
                    NAME = POLICY.PASSWORD_POLICY.Notify,
                    VALUE = 80
                },
                //パスワード再使用禁止期間
                new POLICY{
                    NAME = POLICY.PASSWORD_POLICY.Reuse,
                    VALUE = 8
                }
            };
            var now = DateTimeOffset.Now;

            //無ければ追加する
            var db = serviceProvider.GetRequiredService<RazorPagesLearningContext>();
            foreach (var ele in vals)
            {
                var r = db.POLICies.Where(e => e.NAME == ele.NAME).FirstOrDefault();
                if (null == r)
                {
                    ele.UPDATED_AT = now;
                    ele.CREATED_AT = now;
                    ele.CREATED_USER_ACCOUNT_ID = 0;
                    ele.UPDATED_USER_ACCOUNT_ID = 0;
                    db.POLICies.Add(ele);
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// ドメインデータを初期化する
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void initDOMAINData(IServiceProvider serviceProvider)
        {
            var vals = new List<DOMAIN> {
                //ドメイン(DOMAIN).xlsxの「C#DB定義」列から値をコピーして貼り付ける
                new DOMAIN {CODE="1",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE="フィルム"},
                new DOMAIN {CODE="2",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE="ビデオ"},
                new DOMAIN {CODE="3",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE="その他"},
                new DOMAIN {CODE="4",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE="オーディオ"},
                new DOMAIN {CODE="5",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="6",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE="機材"},
                new DOMAIN {CODE="7",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="8",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="9",JAPANESE_KIND="区分1",KIND="00010002",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="1",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="原版"},
                new DOMAIN {CODE="2",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="素材"},
                new DOMAIN {CODE="3",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="その他"},
                new DOMAIN {CODE="4",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="コピー原版"},
                new DOMAIN {CODE="5",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="ＥＭ"},
                new DOMAIN {CODE="6",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="プリント"},
                new DOMAIN {CODE="7",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE="ＣＯＰＹ"},
                new DOMAIN {CODE="8",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="9",JAPANESE_KIND="区分2",KIND="00010003",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="10",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="登録待"},
                new DOMAIN {CODE="20",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="在庫中"},
                new DOMAIN {CODE="30",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="出荷中"},
                new DOMAIN {CODE="40",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="廃棄済"},
                new DOMAIN {CODE="50",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="抹消済"},
                new DOMAIN {CODE="60",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="依頼中"},
                new DOMAIN {CODE="70",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="複数品"},
                new DOMAIN {CODE="80",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="資材"},
				new DOMAIN {CODE="90",JAPANESE_KIND="ステータス",KIND="00010004",VALID_FLAG=true,VALUE="ゼロ在庫を表示する"},
                new DOMAIN {CODE="10",JAPANESE_KIND="著作権",KIND="00010005",VALID_FLAG=true,VALUE="当社"},
                new DOMAIN {CODE="20",JAPANESE_KIND="著作権",KIND="00010005",VALID_FLAG=true,VALUE="クライアント"},
                new DOMAIN {CODE="30",JAPANESE_KIND="著作権",KIND="00010005",VALID_FLAG=true,VALUE="共著"},
                new DOMAIN {CODE="40",JAPANESE_KIND="著作権",KIND="00010005",VALID_FLAG=true,VALUE="制作委員会"},
                new DOMAIN {CODE="50",JAPANESE_KIND="著作権",KIND="00010005",VALID_FLAG=true,VALUE="広告・代理店・当社"},
                new DOMAIN {CODE="60",JAPANESE_KIND="著作権",KIND="00010005",VALID_FLAG=true,VALUE="不明"},
				new DOMAIN {CODE="10",JAPANESE_KIND="契約書",KIND="00010006",VALID_FLAG=true,VALUE="双方で管理"},
				new DOMAIN {CODE="20",JAPANESE_KIND="契約書",KIND="00010006",VALID_FLAG=true,VALUE="当社のみ補完"},
				new DOMAIN {CODE="30",JAPANESE_KIND="契約書",KIND="00010006",VALID_FLAG=true,VALUE="先方のみ頬間"},
				new DOMAIN {CODE="40",JAPANESE_KIND="契約書",KIND="00010006",VALID_FLAG=true,VALUE="不明"},
                new DOMAIN {CODE="10",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="データ化(暫定)"},
                new DOMAIN {CODE="20",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="データ化(決定)"},
                new DOMAIN {CODE="30",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="廃棄"},
                new DOMAIN {CODE="40",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="返却"},
                new DOMAIN {CODE="50",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="検討中"},
                new DOMAIN {CODE="60",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="テープ以外"},
                new DOMAIN {CODE="70",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="倉庫保管継続"},
                new DOMAIN {CODE="80",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="他部門"},
                new DOMAIN {CODE="90",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="一時保管"},
                new DOMAIN {CODE="100",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="抹消"},
                new DOMAIN {CODE="110",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="データ化済(廃棄予定)"},
                new DOMAIN {CODE="120",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="XDCAM化済(アーカイブ化予定)"},
                new DOMAIN {CODE="130",JAPANESE_KIND="処理判定",KIND="00010007",VALID_FLAG=true,VALUE="アーカイブ化済(倉庫保管継続)"},
                new DOMAIN {CODE="10",JAPANESE_KIND="在庫種別",KIND="00010008",VALID_FLAG=true,VALUE="単品"},
                new DOMAIN {CODE="20",JAPANESE_KIND="在庫種別",KIND="00010008",VALID_FLAG=true,VALUE="複数品"},
                new DOMAIN {CODE="30",JAPANESE_KIND="在庫種別",KIND="00010008",VALID_FLAG=true,VALUE="資材"},
                new DOMAIN {CODE="10",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="在庫中"},
                new DOMAIN {CODE="20",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="出荷中"},
                new DOMAIN {CODE="30",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="廃棄済"},
                new DOMAIN {CODE="40",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="抹消済"},
                new DOMAIN {CODE="50",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="登録待"},
                new DOMAIN {CODE="60",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="依頼中"},
                new DOMAIN {CODE="70",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="複数品"},
                new DOMAIN {CODE="80",JAPANESE_KIND="在庫ステータス",KIND="00010009",VALID_FLAG=true,VALUE="ゼロ在庫を表示する"},
                new DOMAIN {CODE="10",JAPANESE_KIND="処理種別",KIND="00010010",VALID_FLAG=true,VALUE="新規登録"},
                new DOMAIN {CODE="20",JAPANESE_KIND="処理種別",KIND="00010010",VALID_FLAG=true,VALUE="データ取込"},
                new DOMAIN {CODE="30",JAPANESE_KIND="処理種別",KIND="00010010",VALID_FLAG=true,VALUE="再入庫ダイレクト"},
                new DOMAIN {CODE="10",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="新規入庫(登録ユーザ)"},
                new DOMAIN {CODE="20",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="新規入庫"},
                new DOMAIN {CODE="30",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="出荷"},
                new DOMAIN {CODE="40",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="再入庫"},
                new DOMAIN {CODE="50",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="廃棄"},
                new DOMAIN {CODE="60",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="抹消(永久出庫)"},
                new DOMAIN {CODE="70",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="抹消(データ抹消)"},
                new DOMAIN {CODE="80",JAPANESE_KIND="依頼内容",KIND="00020001",VALID_FLAG=true,VALUE="資材販売"},
                new DOMAIN {CODE="A",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="依頼主様取引"},
                new DOMAIN {CODE="B",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="ﾊﾞｲｸ便(TNL手配)"},
                new DOMAIN {CODE="C",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="ﾊﾞｲｸ便(依頼主様手配)"},
                new DOMAIN {CODE="D",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="ヤマト代引"},
                new DOMAIN {CODE="G",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="RazorPagesLearning便"},
                new DOMAIN {CODE="K",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="近鉄物流"},
                new DOMAIN {CODE="M",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="ﾁｬｰﾀｰ便(TNL手配)"},
                new DOMAIN {CODE="S",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="佐川急便"},
                new DOMAIN {CODE="Y",JAPANESE_KIND="便",KIND="00080001",VALID_FLAG=true,VALUE="ヤマト運輸"},
                new DOMAIN {CODE="1",JAPANESE_KIND="指定日時(集配先の便)",KIND="00080002",VALID_FLAG=true,VALUE="指定なし"},
                new DOMAIN {CODE="2",JAPANESE_KIND="指定日時(集配先の便)",KIND="00080002",VALID_FLAG=true,VALUE="ＡＭ"},
                new DOMAIN {CODE="3",JAPANESE_KIND="指定日時(集配先の便)",KIND="00080002",VALID_FLAG=true,VALUE="ＰＭ"},
                new DOMAIN {CODE="0",JAPANESE_KIND="表示",KIND="00080003",VALID_FLAG=true,VALUE=""},
                new DOMAIN {CODE="10",JAPANESE_KIND="表示",KIND="00080003",VALID_FLAG=true,VALUE="ON"},
                new DOMAIN {CODE="20",JAPANESE_KIND="表示",KIND="00080003",VALID_FLAG=true,VALUE="OFF"},
                new DOMAIN {CODE="1",JAPANESE_KIND="WMS状態",KIND="00090000",VALID_FLAG=true,VALUE="作業中"},
                new DOMAIN {CODE="2",JAPANESE_KIND="WMS状態",KIND="00090000",VALID_FLAG=true,VALUE="作業済"},
                new DOMAIN {CODE="9",JAPANESE_KIND="WMS状態",KIND="00090000",VALID_FLAG=true,VALUE="キャンセル"},
                new DOMAIN {CODE="10",JAPANESE_KIND="ステータス(集配依頼)",KIND="00100000",VALID_FLAG=true,VALUE="入力可能"},
                new DOMAIN {CODE="20",JAPANESE_KIND="ステータス(集配依頼)",KIND="00100000",VALID_FLAG=true,VALUE="確定済み"},
                new DOMAIN {CODE="30",JAPANESE_KIND="ステータス(集配依頼)",KIND="00100000",VALID_FLAG=true,VALUE="修正依頼中"},
                new DOMAIN {CODE="40",JAPANESE_KIND="ステータス(集配依頼)",KIND="00100000",VALID_FLAG=true,VALUE="削除"},
                new DOMAIN {CODE="1",JAPANESE_KIND="メールテンプレート",KIND="00110000",VALID_FLAG=true,VALUE="作業依頼履歴確定"},
            };

            //無ければ追加する
            var db = serviceProvider.GetRequiredService<RazorPagesLearningContext>();
            foreach (var ele in vals)
            {
                var r = db.DOMAINs.Where(e => (e.CODE == ele.CODE) &&
                (e.JAPANESE_KIND == ele.JAPANESE_KIND) &&
                (e.KIND == ele.KIND) &&
                (e.VALUE == ele.VALUE)).FirstOrDefault();
                if (null == r)
                {
                    db.DOMAINs.Add(ele);
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// システム設定を追加する
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void initSystemSetting(IServiceProvider serviceProvider)
        {
            //システム設定を追加
            var db = serviceProvider.GetRequiredService<RazorPagesLearningContext>();

            //Mailアドレスの設定
            if (0 == db.SYSTEM_SETTINGs.Count()) {
                db.SYSTEM_SETTINGs.Add( new SYSTEM_SETTING {
                    //ToDo : 正式なアドレスに置換する
                    MAIL_SERVER = "",
                    MAIL_PORT = 0,
                    ADMIN_MAIL = ""
                } );

                db.SaveChanges();
            }

            //Mailテンプレートの設定
            if (0 == db.MAIL_TEMPLATEs.Count())
            {
                db.MAIL_TEMPLATEs.Add(new MAIL_TEMPLATE {
                    MAIL_TEMPLATE_CODE = "1",
                    TEXT = @"写）@Model.wkDelivery.DELIVERY_ADMIN.COMPANY
　　@Model.wkDelivery.DELIVERY_ADMIN.DEPARTMENT
　　@Model.wkDelivery.DELIVERY_ADMIN.CHARGE_NAME  様

@Model.userAccount.COMPANY
@Model.userAccount.DEPARTMENT
@Model.userAccount.NAME  様

この度は TRINET WEB LIBRARY をご利用いただき、誠にありがとう御座います。
下記の通り、ご依頼を承りましたのでお知らせいたします。
※当メールは送信専用メールアドレスから配信されております。
　ご返信いただいてもお答えできませんのでご了承ください。
======================================================================
■集荷先／納品先
　@Model.userAccount.COMPANY
　@Model.userAccount.DEPARTMENT
　@Model.userAccount.NAME  様

　〒@Model.wkDelivery.DELIVERY_ADMIN.ZIPCODE
　@Model.wkDelivery.DELIVERY_ADMIN.ADDRESS1
　@Model.wkDelivery.DELIVERY_ADMIN.ADDRESS2

　TEL:@Model.wkDelivery.DELIVERY_ADMIN.TEL
　FAX:@Model.wkDelivery.DELIVERY_ADMIN.FAX

======================================================================
■御依頼主
　@Model.userAccount.COMPANY
　@Model.userAccount.DEPARTMENT
　@Model.userAccount.NAME  様

　@Model.userAccount.MAIL
　TEL:@Model.userAccount.TEL
　FAX:@Model.userAccount.FAX

======================================================================
■集配予定日
　@Model.wkDelivery.FormattedSPECIFIED_TIME()

@if ((null != @Model.wkDelivery.COMMENT) &&
    (String.Empty != @Model.wkDelivery.COMMENT)){
<text>
======================================================================
■コメント	
</text>
　@Model.wkDelivery.COMMENT
}
======================================================================
■明細
　合計 @Model.wkDelivery.WK_REQUEST.REQUEST_COUNT_SUM 本

SEQ　倉庫管理番号                お客様管理番号               形状
   　題名
   　副題
@foreach (var item in @Model.tableRows.Select((v, i) => new { v, i })){
<text>----------------------------------------------------------------------</text>
@item.i<text>　</text>@item.v.data.STOCK.STORAGE_MANAGE_NUMBER<text>　</text>@item.v.data.STOCK.CUSTOMER_MANAGE_NUMBER<text>　</text>@item.v.data.STOCK.SHAPE<text></text>
<text>   　</text>@item.v.data.STOCK.TITLE<text></text>
<text>   　</text>@item.v.data.STOCK.SUBTITLE<text></text>
}

----------------------------------------------------------------------


======================================================================
■お問合せ先
　三井物産グローバルロジスティクス株式会社
　メディア事業室
　   TEL : 03-5605-8287
  E-mail : media@mitsui-gl.com
"
                });

                db.SaveChanges();

            }

        }

        /// <summary>
        /// デバッグ用のデータを追加する
        /// </summary>
        public static async Task<bool> setupDebugDate(IServiceProvider serviceProvider)
        {

#region ローカル関数

            //運送会社情報を追加する
            void LF_addTRUCK_ADMIN(RazorPagesLearningContext ref_db , string code , string name)
            {
                {
#region 運送会社データを追加
                    var transportAdminService = new RazorPagesLearning.Service.DB.TransportAdminService(ref_db, null, null, null);
                    var r = transportAdminService.read(code).FirstOrDefault();
                    if (null == r)
                    {
                        //運送会社情報を生成
                        var tt = new TRANSPORT_ADMIN
                        {
                            CODE = code,
                            NAME = name,
                            CREATED_USER_ACCOUNT_ID = 0,
                            UPDATED_USER_ACCOUNT_ID = 0
                        };

                        ref_db.TRANSPORT_ADMINs.Add(tt);

                        //[ToDo]
                        //車両マスタを先に入れててく
                        //この作り方でよいか確認が必要。
                        {
                            var tl = new List<TRUCK_ADMIN>();
                            foreach (int i in Enumerable.Range(1, 10))
                            {
                                tl.Add(new TRUCK_ADMIN
                                {
                                    TRUCK_MANAGE_NUMBER = i,
                                    TRANSPORT_ADMIN_CODE = tt.CODE,
                                    TRANSPORT_ADMIN = tt
                                });
                            }

                            tt.TRUCK_ADMINs = tl;
                        }


                        //集配依頼情報を生成する
                        {
                            if (null == tt.DELIVERY_REQUESTs)
                            {
                                tt.DELIVERY_REQUESTs = new List<DELIVERY_REQUEST>();
                            }

                            var statusInde = 1;
                            foreach (int i in Enumerable.Range(1, 100))
                            {
                                var d = new DELIVERY_REQUEST
                                {
                                    DELIVERY_REQUEST_NUMBER = i.ToString(),
                                    STATUS = (statusInde * 10).ToString(),
                                    DELIVERY_DATE = DateTimeOffset.Now,
                                    CONFIRM_DATETIME = DateTimeOffset.Now
                                };
                                ///1から4しかないので補正
                                statusInde = statusInde + 1;
                                if (statusInde > 4)
                                {
                                    statusInde = 1;
                                }

                                //集配詳細情報を追加する
                                d.DELIVERY_REQUEST_DETAILs = new List<DELIVERY_REQUEST_DETAIL>();
                                foreach (int j in Enumerable.Range(1, 10))
                                {
                                    d.DELIVERY_REQUEST_DETAILs.Add(new DELIVERY_REQUEST_DETAIL
                                    {
										DELIVERY_REQUEST_DETAIL_NUMBER = j.ToString(),
										CARGO_AC = "CARGO_AC" + j.ToString(),
                                        CARGO_NOTE = "CARGO_NOTE" + j.ToString(),
                                        CARGO_TITLE = "CARGO_TITLE" + j.ToString(),
                                        COMPANY = "COMPANY" + j.ToString(),
                                        DELIVERY_AC = "DELIVERY_AC" + j.ToString(),
                                        DELIVERY_NOTE = "DELIVERY_NOTE" + j.ToString(),
                                        DELIVERY_TITLE = "DELIVERY_TITLE" + j.ToString(),
                                        BARCODE = "Test"+j.ToString()
                                    });
                                }

                                tt.DELIVERY_REQUESTs.Add(d);
                            }

                        }
                        transportAdminService.add(new List<TRANSPORT_ADMIN> { tt }).Wait();
                    }

#endregion
                }
                ref_db.SaveChanges();
            }

#endregion

            var db = serviceProvider.GetRequiredService<RazorPagesLearningContext>();

            using (var transaction = db.Database.BeginTransaction())
            {

                //荷主マスタを追加する
                {
#region 荷主マスタを追加
                    var shipperAdminService = new RazorPagesLearning.Service.DB.ShipperAdminService(db, null, null, null);
                    {
                        var r = shipperAdminService.read(new Service.DB.ShipperAdminService.ReadConfig
                        {
                            SHIPPER_CODE = "001"
                        });
                        if (null == r.result)
                        {
                            var l = new List<SHIPPER_ADMIN>();
                            l.Add(new SHIPPER_ADMIN
                            {
                                SHIPPER_NAME = "荷主1",
                                SHIPPER_CODE = "001",

                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0
                            });
                            shipperAdminService.add(l);
                        }
                    }

                    {
                        var r = shipperAdminService.read(new Service.DB.ShipperAdminService.ReadConfig
                        {
                            SHIPPER_CODE = "002"
                        });
                        if (null == r.result)
                        {
                            var l = new List<SHIPPER_ADMIN>();
                            l.Add(new SHIPPER_ADMIN
                            {
                                SHIPPER_NAME = "荷主2",
                                SHIPPER_CODE = "002",

                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0
                            });
                            shipperAdminService.add(l);
                        }
                    }

                    {
                        var r = shipperAdminService.read(new Service.DB.ShipperAdminService.ReadConfig
                        {
                            SHIPPER_CODE = "003"
                        });
                        if (null == r.result)
                        {
                            var l = new List<SHIPPER_ADMIN>();
                            l.Add(new SHIPPER_ADMIN
                            {
                                SHIPPER_NAME = "荷主3",
                                SHIPPER_CODE = "003",

                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0
                            });
                            shipperAdminService.add(l);
                        }
                    }

#endregion
                }
                db.SaveChanges();

                {
#region 部課マスタを追加
                    var departmentAdminService = new RazorPagesLearning.Service.DB.DepartmentAdminService(db, null, null, null);
                    {
                        var r = departmentAdminService.read(new Service.DB.DepartmentAdminService.ReadConfig
                        {
                            SHIPPER_CODE = "001",
                            //DEPARTMENT_CODE = "001"
                        });
                        if (null == r.result.FirstOrDefault())
                        {
                            var l = new List<DEPARTMENT_ADMIN>();
                            l.Add(new DEPARTMENT_ADMIN
                            {
                                SHIPPER_CODE = "001",
                                DEPARTMENT_CODE = "011",
                                DEPARTMENT_NAME = "企画部",
                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0
                            });
                            l.Add(new DEPARTMENT_ADMIN
                            {
                                SHIPPER_CODE = "001",
                                DEPARTMENT_CODE = "012",
                                DEPARTMENT_NAME = "映像部",
                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0

                            });
                         departmentAdminService.add(l);
                        }
                        db.SaveChanges();
                    }

                    if (false)
                    {
                        //var r = departmentAdminService.read(new Service.DB.DepartmentAdminService.ReadConfig
                        //{
                        //    SHIPPER_CODE = "002",
                        //    DEPARTMENT_CODE = "001"
                        //});
                        //if (null == r.result.FirstOrDefault())
                        //{
                        //    var l = new List<DEPARTMENT_ADMIN>();
                        //    l.Add(new DEPARTMENT_ADMIN
                        //    {
                        //        SHIPPER_CODE = "002",
                        //        DEPARTMENT_CODE = "011",
                        //        DEPARTMENT_NAME = "癒し部",
                        //        CREATED_USER_ACCOUNT_ID = 0,
                        //        UPDATED_USER_ACCOUNT_ID = 0

                        //    });
                        //    l.Add(new DEPARTMENT_ADMIN
                        //    {
                        //        SHIPPER_CODE = "002",
                        //        DEPARTMENT_CODE = "012",
                        //        DEPARTMENT_NAME = "さぼり部",
                        //        CREATED_USER_ACCOUNT_ID = 0,
                        //        UPDATED_USER_ACCOUNT_ID = 0

                        //    });
                        //    departmentAdminService.add(l);
                        //}
                    }

#endregion
                }

                //運送会社情報追加する
                {
                    LF_addTRUCK_ADMIN(db, "001", "ミヤマ");
                    LF_addTRUCK_ADMIN(db, "002", "クロネコ");
                    LF_addTRUCK_ADMIN(db, "003", "シロネコ");
                }

                {
                    //部課情報関連付け用
                    var departmentAdminService = new RazorPagesLearning.Service.DB.DepartmentAdminService(db, null, null, null);
                    var departmentAdmin = departmentAdminService.read(new Service.DB.DepartmentAdminService.ReadConfig
                    {
                        SHIPPER_CODE = "001"
                    }).result.First();

#region 在庫データをいれる
                    //const int DATA_NUM = 1000000;
                    const int DATA_NUM = 1000;
                    //const int DATA_NUM = 100;
                    const int STEP = 10;
                    if (DATA_NUM > db.STOCKs.Count())
                    {
                        for (int i = 0; i < STEP; i++)
                        {
                            db.STOCKs.AddRange(Enumerable.Range(0, DATA_NUM / STEP).Select(e =>
                            {
                                var id = e.ToString();
                                var now = DateTimeOffset.Now;
                                var userId = 0;
                                return new STOCK
                                {
                                    STATUS = id,
                                    STORAGE_MANAGE_NUMBER = id,
                                    CUSTOMER_MANAGE_NUMBER = id,
                                    TITLE = id,
                                    SUBTITLE = id,
                                    DEPARTMENT_ADMIN = departmentAdmin,
                                    SHAPE = id,
                                    CLASS1 = id,
                                    CLASS2 = id,
                                    REMARK1 = id,
                                    REMARK2 = id,
                                    NOTE = id,
                                    SHIPPER_NOTE = id,
                                    PRODUCT_DATE = "2018",
                                    STORAGE_DATE = now,
                                    PROCESSING_DATE = now,
                                    SCRAP_SCHEDULE_DATE = now,
                                    SHIP_RETURN_CODE = id,
                                    TIME1 = id,
                                    STOCK_COUNT = e,
                                    STORAGE_RETRIEVAL_DATE = now,
                                    ARRIVAL_TIME = id,
                                    BARCODE = id,
                                    STOCK_KIND = id,
                                    WMS_REGIST_DATE = now,
                                    WMS_UPDATE_DATE = now,
                                    CREATED_AT = now,
                                    UPDATED_AT = now,
                                    CREATED_USER_ACCOUNT_ID = userId,
                                    UPDATED_USER_ACCOUNT_ID = userId,
                                    DELETE_FLAG = false
                                };
                            }));

                            db.SaveChanges();
                        }
                    }
#endregion

#region 在庫データ(資材)をいれる
					foreach (int i in Enumerable.Range(1, 10))
					{
						var now = DateTimeOffset.Now;
						var userId = 0;
						var title = "テスト資材" + i.ToString();
						var r = db.STOCKs.Where(e => e.TITLE == title).FirstOrDefault();
						if (null == r)
						{
							db.STOCKs.Add(new STOCK
							{
								TITLE = title,
								STATUS = DOMAIN.StockStatusCode.STOCK,
								SUBTITLE = "サイズ" + i.ToString(),
								SHIPPER_CODE = "999",   // TWLでは、999:RazorPagesLearningになっているため真似ておく
								NOTE = "備考" + i.ToString(),
								STOCK_COUNT = i * 50,
								UNIT = i * 100,
								STOCK_KIND = DOMAIN.StockStockKindCode.MATERIAL,
								CREATED_AT = now,
								UPDATED_AT = now,
								CREATED_USER_ACCOUNT_ID = userId,
								UPDATED_USER_ACCOUNT_ID = userId,
								DELETE_FLAG = false
							});
						}
					}
					db.SaveChanges();
#endregion
                }

#if false // 在庫データ(顧客専用項目付き)をいれる
				{
#region 在庫データ(顧客専用項目付き)をいれる
					const int DATA_NUM = 10000;

					int begin = db.STOCKs.Count() + 1;
					int end = begin + DATA_NUM - 1;

					if (end > db.STOCKs.Count())
					{
						for (int i = begin; i < end; i++)
						{
							var id = i.ToString();
							var now = DateTimeOffset.Now;
							var userId = 0;

							var stock = new STOCK
							{
								STATUS = id,
								STORAGE_MANAGE_NUMBER = id,
								CUSTOMER_MANAGE_NUMBER = id,
								TITLE = id,
								SUBTITLE = id,
								DEPARTMENT_CODE = "011",
								SHAPE = id,
								CLASS1 = id,
								CLASS2 = id,
								REMARK1 = id,
								REMARK2 = id,
								NOTE = id,
								SHIPPER_NOTE = "001",
								PRODUCT_DATE = now,
								STORAGE_DATE = now,
								PROCESSING_DATE = now,
								SCRAP_SCHEDULE_DATE = now,
								SHIP_RETURN_CODE = id,
								CUSTOMER_ITEM1_CODE = id,
								CUSTOMER_ITEM1_VALUE = id,
								CUSTOMER_ITEM2_CODE = id,
								CUSTOMER_ITEM2_VALUE = id,
								CUSTOMER_ITEM3_CODE = id,
								CUSTOMER_ITEM3_VALUE = id,
								TIME1 = "001",
								STOCK_COUNT = i,
								STORAGE_RETRIEVAL_DATE = now,
								ARRIVAL_TIME = id,
								BARCODE = id,
								STOCK_KIND = id,
								REGIST_DATE = now,
								UPDATE_DATE = now,
								CREATED_AT = now,
								UPDATED_AT = now,
								CREATED_USER_ACCOUNT_ID = userId,
								UPDATED_USER_ACCOUNT_ID = userId,
								DELETE_FLAG = false,

								CUSTOMER_OPTION_ATTRIBUTE = new CUSTOMER_OPTION_ATTRIBUTE
								{
									PROJECT_NO1 = id,
									PROJECT_NO2 = id,
									COPYRIGHT1 = id,
									COPYRIGHT2 = id,
									CONTRACT1 = id,
									DATA_NO1 = id,
									DATA_NO2 = id,
									PROCESS_JUDGE1 = id,
									PROCESS_JUDGE2 = id,
									CREATED_AT = now,
									UPDATED_AT = now,
									CREATED_USER_ACCOUNT_ID = userId,
									UPDATED_USER_ACCOUNT_ID = userId,
									DELETE_FLAG = false
								}
							};

							db.STOCKs.Add(stock);
							db.SaveChanges();
						}
					}
#endregion
				}
#endif

                {
#region メッセージマスタを追加
                    var messageAdminService = new RazorPagesLearning.Service.DB.MessageAdminService(db, null, null, null);
                    {
                        var now = DateTimeOffset.Now;

                        // ログイン
                        var r = messageAdminService.Read(new Service.DB.MessageAdminService.ReadConfig
                        {
                            KIND = MESSAGE_ADMIN.MESSAGE_KIND.Login
                        });
                        if (null == r.result)
                        {
                            var e = new MESSAGE_ADMIN
                            {
                                KIND = MESSAGE_ADMIN.MESSAGE_KIND.Login,
                                MESSAGE = "login message",
                                CREATED_AT = now,
                                UPDATED_AT = now,
                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0
                            };
                            db.MESSAGE_ADMINs.Add(e);
                        }

                        // ホーム
                        r = messageAdminService.Read(new Service.DB.MessageAdminService.ReadConfig
                        {
                            KIND = MESSAGE_ADMIN.MESSAGE_KIND.Home
                        });
                        if (null == r.result)
                        {
                            var e = new MESSAGE_ADMIN
                            {
                                KIND = MESSAGE_ADMIN.MESSAGE_KIND.Home,
                                MESSAGE = "home message",
                                CREATED_AT = now,
                                UPDATED_AT = now,
                                CREATED_USER_ACCOUNT_ID = 0,
                                UPDATED_USER_ACCOUNT_ID = 0
                            };
                            db.MESSAGE_ADMINs.Add(e);
                        }

                        db.SaveChanges();
                    }

#endregion
                }

				{
#region 集配先マスタを追加
					// 全荷主にDATA_NUM件ずつ集配先マスタを追加
					const int DATA_NUM = 5;
					var now = DateTimeOffset.Now;
					var userId = 0;

					foreach (var shipperAdmin in db.SHIPPER_ADMINs.ToList())
					{
						var deliveryAdmins = db.DELIVERY_ADMINs.Where(e => e.SHIPPER_CODE == shipperAdmin.SHIPPER_CODE);
						for (int i = deliveryAdmins.Count(); i < DATA_NUM; i++)
						{
							db.DELIVERY_ADMINs.Add(new DELIVERY_ADMIN
							{
								SHIPPER_CODE = shipperAdmin.SHIPPER_CODE,
								COMPANY = $"{shipperAdmin.SHIPPER_NAME}_集配先{i + 1}",
								ADDRESS1 = $"{shipperAdmin.SHIPPER_NAME}_住所{i + 1}",
								TEL = i.ToString(),
								CREATED_AT = now,
								UPDATED_AT = now,
								CREATED_USER_ACCOUNT_ID = userId,
								UPDATED_USER_ACCOUNT_ID = userId,
								DELETE_FLAG = false
							});

							db.SaveChanges();
						}
					}
#endregion
				}

				//======================================================================================
				// WMS集配先選択用の荷主マスタとWMS集配先マスタを追加する
				//======================================================================================
				{
#region WMS集配先選択用の荷主マスタとWMS集配先マスタを追加
                    var shipperAdminService = new RazorPagesLearning.Service.DB.ShipperAdminService(db, null, null, null);
                    {
                        var userService = new RazorPagesLearning.Service.User.UserService
                            (serviceProvider.GetRequiredService<RazorPagesLearningContext>(), null, null, null);

                        //------------------------------------------------------------------------------
                        // ユーザを検索し、なければ追加する
                        //------------------------------------------------------------------------------
                        // ユーザ検索
                        var wReadUser = userService.read("MWLSystemAdmin");
                        // ユーザがなければ
                        if (null == wReadUser)
                        {
                            //認証機構を取得する
                            var user = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                            // ユーザを追加
                            var r = userService.add(
                            new Service.User.UserService.ChangeConfig
                            {
                                password = "MWLSystemAdmin2018",
                                USER_ACCOUNT = new USER_ACCOUNT
                                {
                                    USER_ID = "MWLSystemAdmin",
                                    NAME = "MWLSystemAdmin",
                                    KANA = "MWLSystemAdmin",
                                    COMPANY = "三井物産グローバルロジスティクス株式会社",
                                    ADDRESS1 = "東京都港区東新橋2-14-1",
                                    ZIPCODE = "1050021",
                                    TEL = "0356571130",
                                    FAX = "0356571130",
                                    MAIL = "hoge@test.com",
                                    PERMISSION = USER_ACCOUNT.ACCOUNT_PERMISSION.Admin

                                }
                            });
                            db.SaveChanges();
                        }


                        //------------------------------------------------------------------------------
                        // 荷主マスタを検索し、なければ追加する
                        //------------------------------------------------------------------------------
                        // 荷主マスタ検索
                        var wReadShipperAdmin = shipperAdminService.read(new Service.DB.ShipperAdminService.ReadConfig
                        {
                            SHIPPER_CODE = "N01"
                        });
                        // 荷主マスタがなければ
                        if (null == wReadShipperAdmin.result)
                        {
                            // 荷主マスタを追加
                            var wLstShipperAdmin = new List<SHIPPER_ADMIN>();
                            wLstShipperAdmin.Add(new SHIPPER_ADMIN
                            {
                                SHIPPER_CODE = "N01",           // 荷主コード
                                SHIPPER_NAME = "テレビ001", // 荷主名

                                AFTERNOON_FLAG = true,         // 午後設定
                                PASSWORD_FLAG = true,          // パスワード有効期限
                                CUSTOMER_ONLY_FLAG = true,     // 顧客専用項目
                                CREATED_AT = DateTime.ParseExact("2018/08/07 00:00:00", "yyyy/MM/dd HH:mm:ss", null), // 登録日
                                UPDATED_AT = DateTime.ParseExact("2018/08/07 00:00:00", "yyyy/MM/dd HH:mm:ss", null), // 更新日
                                CREATED_USER_ACCOUNT_ID = 0,   // 登録ユーザーID
                                UPDATED_USER_ACCOUNT_ID = 0,   // 更新ユーザーID
                                DELETE_FLAG = false            // 削除フラグ
                            });
                            shipperAdminService.add(wLstShipperAdmin);

                            db.SaveChanges();
                        }

#if false // 2018/08/16 M.Hoshino del DBマイグレーション
                    //------------------------------------------------------------------------------
                    // WMS集配先マスタを検索し、なければ追加
                    //------------------------------------------------------------------------------
                    // 荷主マスタ検索
                    var wReadParentShipperAdmin = RazorPagesLearning.Service.DB.ShipperAdminService.read(db, new Service.DB.ShipperAdminService.ReadConfig
                    {
                        SHIPPER_CODE = "N01"
                    });

					// WMS集配先マスタ検索
					var wReadWmsDeriveryAdminService = RazorPagesLearning.Service.DB.WmsDeriveryAdminService.Read(db, new Service.DB.WmsDeriveryAdminService.ReadConfig
                    {
                        _DestCode = "WDA_NODA_001"
                    });
					// WMS集配先マスタがなければ
					if (null == wReadWmsDeriveryAdminService.result)
                    {
                        // WMS集配先マスタを追加
                        var wLstWmsDeriveryAdminService = new List<WMS_DERIVERY_ADMIN>();
                        wLstWmsDeriveryAdminService.Add(new WMS_DERIVERY_ADMIN
                        {
                            DEST_CODE = "WDA_NODA_001",        // 集配先コード
//THIS_IS_PENDING
                            SHIPPER_ADMIN = wReadParentShipperAdmin.result,
                                                               // 荷主マスタID
                            COMPANY = "野田テレビ001",         // 社名
                            DEPARTMENT = "第1放送部",          // 部署名
                            CHARGE_NAME = "野田昌宏",          // 担当者名
                            ZIPCODE = "252-0253",              // 郵便番号
                            ADDRESS1 = "神奈川県相模市中央区", // 住所1
                            ADDRESS2 = "南橋本1-20-11-501",    // 住所2
                            TEL = "0427-11-2222",              // TEL
                            FAX = "0427-33-4444",              // FAX
                            FLIGHT_CODE = "FC_NODA_001",       // 便コード
                            MAIL = "m_noda@vega-net.co.jp",    // メールアドレス
                            MAIL1 = "m_noda1@vega-net.co.jp",  // 同時配信メール1
                            MAIL2 = "m_noda2@vega-net.co.jp",  // 同時配信メール1
                            MAIL3 = "m_noda3@vega-net.co.jp",  // 同時配信メール1
                            MAIL4 = null,                      // 同時配信メール1
                            MAIL5 = null,                      // 同時配信メール1
                            MAIL6 = null,                      // 同時配信メール1
                            MAIL7 = null,                      // 同時配信メール1
                            MAIL8 = null,                      // 同時配信メール1
                            MAIL9 = null,                      // 同時配信メール1
                            MAIL10 = null,                     // 同時配信メール1
                            CREATED_AT = DateTime.ParseExact("2018/08/07 00:00:00", "yyyy/MM/dd HH:mm:ss", null), // 登録日時
                            UPDATED_AT = DateTime.ParseExact("2018/08/07 00:00:00", "yyyy/MM/dd HH:mm:ss", null), // 更新日時
                            CREATED_USER_ID = "NODA_001",      // 登録ユーザID
                            UPDATED_USER_ID = "NODA_001"       // 更新ユーザID
                        });
                        RazorPagesLearning.Service.DB.WmsDeriveryAdminService.add(db, wLstWmsDeriveryAdminService);

                        db.SaveChanges();
                    }
#endif // 2018/08/16 M.Hoshino del DBマイグレーション

#if false // 2018/08/16 M.Hoshino del DBマイグレーション
					//------------------------------------------------------------------------------
					// 集配先表示フラグマスタを検索し、なければ追加
					//------------------------------------------------------------------------------
					// ユーザアカウントマスタ検索
					var wReadParentUserService = RazorPagesLearning.Service.User.UserService.read(db, "MWLSystemAdmin");

                    // WMS集配先マスタ検索
                    var wReadParentWmsDeriveryAdminService = RazorPagesLearning.Service.DB.WmsDeriveryAdminService.Read(db, new Service.DB.WmsDeriveryAdminService.ReadConfig
                    {
                        _DestCode = "WDA_NODA_001"
                    });

                    // 集配先表示フラグマスタ検索
                    var wReadDeriveryDispFlagService = RazorPagesLearning.Service.DB.DeriveryDispFlagService.read(db, new Service.DB.DeriveryDispFlagService.ReadConfig
                    {
                        WMS_DERIVERY_ADMIN = wReadParentWmsDeriveryAdminService.result,

                        USER_ACCOUNT = wReadParentUserService

                    });
                    // 集配先表示フラグマスタがなければ
                    if( null == wReadDeriveryDispFlagService.result )
                    {
                        // WMS集配先マスタを追加
                        var wLstDeriveryDispFlagService = new List<DELIVERY_DISP_FLAG>();
                        wLstDeriveryDispFlagService.Add(new DELIVERY_DISP_FLAG
                        {
//THIS_IS_PENGING
                            WMS_DERIVERY_ADMIN = wReadParentWmsDeriveryAdminService.result, //集配先マスタID

                            USER_ACCOUNT = wReadParentUserService, //ユーザID

                            DISPLAY_FLAG = true, //表示フラグ

                            CREATED_AT = DateTime.ParseExact("2018/08/07 00:00:00", "yyyy/MM/dd HH:mm:ss", null), // 登録日時
                            UPDATED_AT = DateTime.ParseExact("2018/08/07 00:00:00", "yyyy/MM/dd HH:mm:ss", null), // 更新日時
                            CREATED_USER_ID = "NODA_001",      // 登録ユーザID
                            UPDATED_USER_ID = "NODA_001"       // 更新ユーザID
                        });
                        RazorPagesLearning.Service.DB.DeriveryDispFlagService.add(db, wLstDeriveryDispFlagService);

                        db.SaveChanges();
                    }
#endif // 2018/08/16 M.Hoshino del DBマイグレーション
                    }

#endregion
                }
                await db.SaveChangesAsync();
                transaction.Commit();
            }

#if false
            //データ生成に時間がかかるので、通常は無効化しておく
            {
#region ユーザーアカウント情報
                var adminId = 0;
                var adminName = "MWLSystemAdmin";
                var adminPass = "MWLSystemAdmin2018";

                //認証機構を取得する
                var user = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var userService = new RazorPagesLearning.Service.User.UserService
                    (serviceProvider.GetRequiredService<RazorPagesLearningContext>(), null, null,
                   user);

                for (int i = 1; i <= 150; i++)
                {
                    var ff = userService.read(adminName + i.ToString());
                    if (null == ff)
                    {
                        //サービスを使ってユーザーを追加する
                        var r = await userService.add(
                        new Service.User.UserService.ChangeConfig
                        {
                            password = adminPass,
                            USER_ACCOUNT = new USER_ACCOUNT
                            {
                                USER_ID = adminName + i.ToString(),
                                NAME = adminName + i.ToString(),
                                KANA = adminName + i.ToString(),
                                COMPANY = "三井物産グローバルロジスティクス株式会社" + i.ToString(),
                                ADDRESS1 = "東京都港区東新橋2-14-1" + i.ToString(),
                                ZIPCODE = "1050021",
                                TEL = "0356571130" + i.ToString(),
                                FAX = "0356571130" + i.ToString(),
                                MAIL = "hoge@test.com" + i.ToString(),
                                PERMISSION = USER_ACCOUNT.ACCOUNT_PERMISSION.Admin,
                                LOGIN_ENABLE_FLAG = true
                            },
                            createdUserAccountId = adminId,
                            updatedUserAccountId = adminId
                        });
                        if (false == r.succeed)
                        {
                            //ここでエラーが出ていたらどうにもならないので例外投げて終了とする。
                            throw new ApplicationException(r.errorMessages.FirstOrDefault());
                        }
                    }
                }

#endregion
            }

#endif
            await db.SaveChangesAsync();

            return true;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<RazorPagesLearningContext>();

            //DBが存在しない場合、
            //EnsureCreated()関数を呼び出すと
            //DBを作成してくれる。
            //存在する場合は何もしない。
            //この関数を呼び出してもマイグレーションは適応されないので注意が必要。
            context.Database.EnsureCreated();

            //ポリシーデータを追加
            initPOLICY(serviceProvider);

            //権限一覧を追加
            initUserRoles(serviceProvider).Wait();

            //システムのデフォルト管理者を登録する
            initAdminUser(serviceProvider).Wait();

            //ドメインデータを追加する
            initDOMAINData(serviceProvider);

            //システム設定情報を追加する
            initSystemSetting(serviceProvider);

            //テスト用の初期DBを追加する
            setupDebugDate(serviceProvider).Wait();


        }
    }
}
