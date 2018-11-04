using RazorPagesLearning.Data;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.Pagination;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Utility.SelectableTable
{
    /// <summary>
    /// 画面上の表領域における、チェック済みチェックボックスの状態、復元を管理する
    /// </summary>
    public abstract class CheckStatusManagement<TableDataType>
    {

        //コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly Service.User.UserService userService;
        private readonly RazorPagesLearning.Service.DB.TableSelectionSettingService tableSelectionSettingService;
        private readonly Service.DB.TablePaginationSettingService tablePaginationSettingService;
        private RazorPagesLearningContext db;
        private SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager;
        private PaginationInfo paginationInfo;

        public CheckStatusManagement(RazorPagesLearning.Data.RazorPagesLearningContext db,
             SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            PaginationInfo paginationInfo,
            ClaimsPrincipal ref_user)
        {
            //this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;

            this.userService = new Service.User.UserService(db, ref_user, signInManager, userManager);
            this.tableSelectionSettingService = new TableSelectionSettingService(db, ref_user, signInManager, userManager);
            this.tablePaginationSettingService = new TablePaginationSettingService(db, ref_user, signInManager, userManager);
        }

        protected CheckStatusManagement(RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, PaginationInfo paginationInfo)
        {
            this.db = db;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.paginationInfo = paginationInfo;
        }

        /// <summary>
        /// 表領域において、チェック状態を保存するテーブルと、
        /// 実際のデータが保存されているテーブル間でキーを関連付けるするための関数
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public abstract TableSelectionSettingService.TrackingIdentifier translationRelationIdentifier(TableDataType table);

        /// <summary>
        /// 状態保存設定
        /// </summary>
        public class StatePersistenceConfig
        {
            /// <summary>
            /// ユーザー情報
            /// </summary>
            public ClaimsPrincipal userInfo;

            //-- -- -- --
            //Memo
            //
            //viewModelとpaginationInfoは画面描画時にpostデータから復元される。
            //このため、関数呼び出し時には毎回別のオブジェクトが入っていることとなる。
            //CheckStatusManagement側のメンバ変数ではなく、
            //関数の呼び出し時に毎度与えることとする。

            /// <summary>
            /// 表示対象のデータ
            /// </summary>
            public SelectableTableViewModelBase<TableDataType> viewModel;

            /// <summary>
            /// ページネーション情報
            /// </summary>
            public PaginationInfo paginationInfo;
        }

        /// <summary>
        ///　表上の選択状態を記憶する
        /// </summary>
        public async Task<bool> stateSave(StatePersistenceConfig config)
        {
            var _db = this.userService.db;
            //同一トランザクションで処理するために使用するDBコンテクストを更新する
            this.tableSelectionSettingService.updateRazorPagesLearningContext(_db);
            this.tablePaginationSettingService.updateRazorPagesLearningContext(_db);
            using (var transaction = _db.Database.BeginTransaction())
            {
                //現在ログイン中のユーザーを取得する
                var user = await this.userService.read(
                new ReadFromClaimsPrincipalConfig
                {
                    userInfo = config.userInfo
                });
                if (null != user)
                {
                    ///ユーザーのチェックボックスチェック状態、付加情報の内容を保存する
                    {
                        foreach (var ele in config.viewModel.tableRows)
                        {
                            var t = this.tableSelectionSettingService.read(
                                new Service.DB.TableSelectionSettingService.ReadConfig
                                {
                                    trackingIdentifier = ele.trackingIdentifier,
                                    USER_ACCOUNT_ID = user.ID,
                                    viewTableType = config.viewModel.viewTableType
                                });
                            if (null == t)
                            {
                                //新規作成してデータを追加する
                                t = this.tableSelectionSettingService.addNew(
                                    user,
                                    ele.trackingIdentifier,
                                    config.viewModel.viewTableType
                                    );
                            }

                            // チェックボックス状態
                            t.selected = ele.isSelected;
                            // 付加情報
                            t.appendInfo = ele.appendInfo;
                        }
                        _db.SaveChanges();
                    }

                    //ページネーション関係の全体情報を保存する
                    {
                        var t = tablePaginationSettingService.read(
                            new Service.DB.TablePaginationSettingService.ReadConfig
                            {
                                USER_ACCOUNT_ID = user.ID,
                                viewTableType = config.viewModel.viewTableType
                            });
                        if (null == t)
                        {
                            t = new Data.Models.WK_TABLE_PAGINATION_SETTING
                            {
                                viewTableType = config.viewModel.viewTableType
                            };
                            user.WK_TABLE_PAGINATION_SETTINGs.Add(t);
                        }
                        else
                        {
                            //状態が変わっていたら、関係する項目全てを上書き更新する。
                            if (t.checkAllPage != config.paginationInfo.checkAllPage)
                            {
                                //状態が変わっているので変更更新処理を行う
                                //DB上に存在する履歴レコードを全て上書きする
                                tableSelectionSettingService.setAllIsSelectedState(
                                    new Service.DB.TableSelectionSettingService.UpdateConfig
                                    {
                                        USER_ACCOUNT_ID = user.ID,
                                        viewTableType = config.viewModel.viewTableType,
                                        selected = config.paginationInfo.checkAllPage
                                    });
							}
						}

                        t.checkAllPage = config.paginationInfo.checkAllPage;
                        _db.SaveChanges();
                    }

                    transaction.Commit();
				}
			}
            return true;
        }

        
        /// <summary>
        /// 状態復元関数
        /// </summary>
        public class StateRestoreConfig : StatePersistenceConfig
        {
            /// <summary>
            /// DBから情報を読み取るカラム
            /// </summary>
            public Func<IQueryable<TableDataType>> readFunc;

            /// <summary>
            /// サーバー上で選択済みのチェックボックス件数
            /// </summary>
            public int maxRecords;

        }

        /// <summary>
        /// 表に表示する情報を読み取ってviewモデルに戻す。
        /// 合わせて、表上のチェック状態も復元する。
        /// </summary>
        public async Task<bool> stateRestore(StateRestoreConfig config)
        {
            //現在ログイン中のユーザーを取得する
            var user = await this.userService.read(
            new ReadFromClaimsPrincipalConfig
            {
                userInfo = config.userInfo
            });
            if (null != user)
            {
                config.viewModel.tableRows =
                    config.readFunc()
                    .ToList()
                    .Select(e =>
                    {
                        //DB上での該当テーブル追跡列情報に変換する
                        //複合キーになる事を考慮して一旦オブジェクトを経由する
                        var trackingIdentifier = translationRelationIdentifier(e);

                        var isSelected = false;
                        var appendInfo = "";
                        {
				#region 選択状態を復元
                            var t = tableSelectionSettingService.read(
                                    new Service.DB.TableSelectionSettingService.ReadConfig
                                    {
                                        trackingIdentifier = trackingIdentifier,
                                        USER_ACCOUNT_ID = user.ID,
                                        viewTableType = config.viewModel.viewTableType
                                    });
                            if (null != t)
                            {
                                isSelected = t.selected;
                                appendInfo = t.appendInfo;
                            }
				#endregion
                        }

                        return new RowInfo<TableDataType>
                        {
                            trackingIdentifier = trackingIdentifier,
                            isSelected = isSelected,
                            appendInfo = appendInfo,
                            data = e
                        };
                    }
                    )
                    .ToList();

                {
				#region　サーバー上で選択されているデータ件数を取得する

                    //[Memo]
                    //本システムでは選択ページ数をサーバー上に残す。
                    //このため、ページ上の選択状態は以下挙動をとることとなる。
                    //
                    //サーバー上 : DBに登録されている情報
                    //クライアント上 : 画面上でチェックボックスの選択状態は常時変更できる。
                    //
                    //このため、サーバーからクライアントへ返す選択状態は
                    //画面表示されていないページの選択状態を返す。
                    //画面表示されているページに関しては、JS上で選択状態を確認して返す
                    if (true == config.paginationInfo.checkAllPage)
                    {
                        //全ページを選択状態であれば、
                        //現在のページ以外の全件数は選択されているものと想定する。
                        config.viewModel.checkedCountOnServer = config.maxRecords - config.viewModel.tableRows.Count();
                    }
                    else
                    {
                        //画面に表示するデータを除いて、
                        //選択済みのデータ件数を取得する
                        config.viewModel.checkedCountOnServer =
                            tableSelectionSettingService.readExcludedId(
                            new Service.DB.TableSelectionSettingService.ReadExcludedConfig
                            {
                                excludedIds = config.viewModel.tableRows.Select(e => e.trackingIdentifier),
                                USER_ACCOUNT_ID = user.ID,
                                viewTableType = config.viewModel.viewTableType
                            })
                            .Where(e => e.selected == true)
                            .Count();
                    }
				#endregion
                }

                {
                    var paginationSetting = tablePaginationSettingService.read(
                        new Service.DB.TablePaginationSettingService.ReadConfig
                        {
                            USER_ACCOUNT_ID = user.ID,
                            viewTableType = Data.Models.ViewTableType.Search
                        });
                    if (null != config.paginationInfo)
                    {
                        if (true == config.paginationInfo.checkAllPage)
                        {

                        }
                    }
                }

                return true;
			}
			else
            {
                return false;
            }
        }

    }
}
