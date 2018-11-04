//MGLシステムにおいて、ベガ社内で使用する
//JS処理をまとめた実装空間

//ベガ関係の処理実装ユーティリティ
var VegaUtility = function () { };

//静的関数群
VegaUtility.StaticFunctions = function () { };

// -- -- * -- -- *  -- -- *  -- -- 
//その他補助関数
VegaUtility.StaticFunctions.Helpers = {

    //文字列形式でなどのほか形式で表現されたbooleanを
    //通常のboolean形式に変換する
    parseBoolean: function (value) {
        if (typeof value === "boolean") return value;

        if (typeof value === "number") {
            return value === 1 ? true : value === 0 ? false : undefined;
        }

        if (typeof value != "string") return undefined;

        return value.toLowerCase() === 'true' ? true : false;
    }
};
// -- -- * -- -- *  -- -- *  -- --

// -- -- * -- -- *  -- -- *  -- -- 
//ローディングを表示する
VegaUtility.StaticFunctions.LoadingSplash =
    {
        //表示する
        show: function () {
            var height = $(window).height(); //ブラウザウィンドウの高さを取得            
            $('#loader-bg ,#loader').height(height).css('display', 'block'); //ウィンドウの高さに合わせでローディング画面を表示
        },
        //要素を隠す
        hide: function () {
            $('#loader-bg').delay(900).fadeOut(800);
            $('#loader').delay(600).fadeOut(300);
        }
    }
// -- -- * -- -- *  -- -- *  -- --

// -- -- * -- -- *  -- -- *  -- -- 
//ポップアップ関係
VegaUtility.StaticFunctions.PopUp =
    {
        //表示する
        show: function (popName, autoFlag) {
            document.location.href = popName;
            //モーダル画面を表示中フラグを立てておく
            if (null == autoFlag) {
                var ele = $("[ id$=isShowModal]");
                if (null != ele) {
                    ele.val(true);
                }
            }
        }

    }
// -- -- * -- -- *  -- -- *  -- --

// -- -- * -- -- *  -- -- *  -- -- 
//ASP 処理の補助関数群ヘルパー関数群
VegaUtility.StaticFunctions.ASPHelper =
    {
        //チェックボックスの選択状態が変更されたら
        //該当要素のvalue値を選択状態に合わせて設定する。
        //この処理を挟まないと、postされる時に画面上の値が正しく反映されていないので、
        //この処理を通して更新をかける。
        registerEvent_SynchronizeTheCheckboxValue: function (delegationFunc) {

            $(':checkbox').on('change', function (e) {

                //チェックボックスの選択状態が変更されたら
                //該当要素のvalue値を選択状態に合わせて設定する。
                //この処理を挟まないと、postされる時に画面上の値が正しく反映されていないので、
                //この処理を通して更新をかける。                
                var checkedVal = $(this).prop("checked");
                $(this).val(checkedVal);
                $(this).attr('data-val', checkedVal);
                if (false == checkedVal) {
                    $(this).removeAttr('checked');
                }

                //ASP Razorの場合、asp-forでcheckboxを作成すると、
                //それと同一名称のhidden要素を作ってそちら側にチェック結果を保存している。
                //画面上で要素チェックが行われたら、hidden要素側もその値に合わせておく
                var name = $(this).attr('id');
                var hiddenT = $('input:hidden' + "#hidden-" + name);
                hiddenT.val(checkedVal);
                hiddenT.attr('data-val', checkedVal);

                try {

                    //テーブルの表において
                    //チェック済みのチェックボックス一覧を取得する
                    (function () {

                        //選択件数
                        VegaUtility.StaticFunctions.TableHelper.StaticVariable.checkedRows =
                            VegaUtility.StaticFunctions.TableHelper.checkCount();

                    })();

                    //画面上のチェック済み件数を更新する
                    VegaUtility.StaticFunctions.TableHelper.updateCheckCount();

                } catch(e){
                    //DataTablesの定義が存在しないと落ちるので、
                    //その場合は無視する
                }

                //処理完了時にデリゲーション関数が張ってあったら呼び出しを行う
                if (null != delegationFunc) {
                    delegationFunc();
                }

            });
        }
    }

// -- -- * -- -- *  -- -- *  -- -- 
//テーブル関係の処理をまとめて実装する
VegaUtility.StaticFunctions.TableHelper =
    {
        //共通で使用するグローバル変数
        StaticVariable: {
            //チェック済みの行数
            checkedRows: 0
        },
        //現在のページに存在するチェックボックスの値を一括で変更する
        setThisPageCheckStatus: function (target) {

            var checkedVal = $(target).prop("checked");

            //表の中の選択要素をすべて変更する
            //idにcheckbox_isSelectedを持っている
            $("[id*=checkbox_isSelected]").each(function (index, ele) {
                var se = $(ele);
                se.prop("checked", checkedVal);
                se.val(checkedVal);
            });

            //選択済み状態を更新
            VegaUtility.StaticFunctions.TableHelper.StaticVariable.checkedRows =
                VegaUtility.StaticFunctions.TableHelper.checkCount();
        },
        //テーブル上でのチェック済みのチェックボックス数を取得する
        checkCount: function () {

            try {

                //DataTablesにおいて、横列を固定すると、固定内容と表示内容でテーブルが内部的に複製される。
                //チェック済み一覧の検査対象となる表は「dataTables_scroll」クラスが定義されているので、
                //そちら側の列に定義されている値を取得する。
                //var hiddenCheckbox = $('.DTFC_LeftWrapper input:hidden' + "[id^=hidden-checkbox]");
                var hiddenCheckbox = $('.dataTables_scroll input:hidden' + "[id^=hidden-checkbox]");
                var checkedList = hiddenCheckbox.filter(function (index, element) {
                    var vv = $(element).val();
                    console.log(vv);
                    var v3 = VegaUtility.StaticFunctions.Helpers.parseBoolean(vv);
                    return v3;
                });

                return checkedList.length;

            } catch(e){
                //dataTablesがない状態で動く場合ね考えられるので、
                //失敗したら無視する
            }

            return 0;
        },
        //テーブル上での選択済みの件数を画面上に更新する
        updateCheckCount: function () {

            try {
                //サーバー所の選択件数を取得する
                var checkedCountOnServer = Number($("#dataTables-checkedCountOnServer").val());

                var now = checkedCountOnServer +
                    VegaUtility.StaticFunctions.TableHelper.StaticVariable.checkedRows;

                $("#dataTables-checkedCount").text(now);
            } catch (e) {
                console.log(e);
            }
        },
        /**
        DataTableオブジェクトを共通形式でセットアップする
         引数オブジェクトのフォーマット
    
            var arg = {
                //テーブル要素のID
                tableElementID: "",
                //テーブル共通機能領域のサイズ
                tableCommonFunctionElementID: "",
                //列左端　スクロール固定列数
                leftScrollFixedColumns : 2,
                //列のデフォルトカラムサイズ(省略可)
                columnSize:[                
                    { width: '1rem', targets: 0 },
                    { width: '4rem', targets: 1 }
                ]
            };
            
         */
        setupTable: function (arg) {

            //--- ローカル関数 ---

            //保存用のID名を取得する
            var LF_getStoreID = function (idName) {
                return "save-column-width-size-" + idName;
            };

            //--- ローカル関数 ---

            //列幅を復元するための読み込みを行う
            var tColumnDefs = (function () {

                //左端の列固定設定を決定する
                var ret = (function () {
                    if (null != arg.columnSize) {
                        return arg.columnSize;
                    }
                    else {
                        return [
                            { width: '1rem', targets: 0 },
                            { width: '4rem', targets: 1 }
                        ];
                    }
                })();

                //設定値が残っていたら、その値を採用する
                var tStoreID = LF_getStoreID(arg.tableElementID);
                var dbData = store.get(tStoreID);
                if (null != dbData) {
                    //値が残っていたらので戻す

                    //列の入れ替えを想定して、要素類から列の位置を再計算する
                    $(arg.tableElementID + " th").each(function (index, element) {
                        //テーブルサイズの保存指定を判定
                        var saveInfo = $(element).attr("save-column-width-size-name");
                        if (null != saveInfo) {
                            var size = dbData[saveInfo];
                            if (null != size) {
                                //該当要素が残っていたら値を設定する
                                ret.push(
                                    {
                                        width: size + "px", targets: index
                                    }
                                );
                            }
                        }
                    });
                }
                return ret;
            })();

            //縦方向のサイズを算出
            //スクロールバーを表示するためには、縦要素の最大幅を指定しておく必要あり。
            var hSize = (function () {
                var naviH = $("#Nav").height();
                var h = $(arg.tableCommonFunctionElementID).height();
                return (naviH - h) * 0.9;
            })();
            hSize = $('#Nav_menu').height();

            //指定されたテーブル要素にDataTableを適応
            var table = $(arg.tableElementID).DataTable({
                'dom': 'Rt',
                scrollY: hSize + "px",
                scrollX: true,
                scrollCollapse: true,
                paging: false,
                searching: false,
                info: false,
                columnDefs: tColumnDefs,
                fixedColumns: {
                    leftColumns: arg.leftScrollFixedColumns,
                    rightColumns: 0
                },
                // ソート機能 無効
                ordering: false,
                "drawCallback": function (settings) {

                    //選択件数
                    VegaUtility.StaticFunctions.TableHelper.StaticVariable.checkedRows =
                        VegaUtility.StaticFunctions.TableHelper.checkCount();

                    //選択済みの件数を更新する
                    VegaUtility.StaticFunctions.TableHelper.updateCheckCount();

                    //ローディング表示を閉じる
                    VegaUtility.StaticFunctions.LoadingSplash.hide();

                },
                "preDrawCallback": function (settings) {
                    //ローディング表示を行う
                    VegaUtility.StaticFunctions.LoadingSplash.show();
                }
            }).columns.adjust().draw();

            //ヘッダサイズ変更時に呼び出される関数
            $(arg.tableElementID).on('column-resize.dt.mouseup', function (event, oSettings) {
                // 列サイズが変更されたときにここが呼び出される

                //ヘッダサイズ
                var tableHeaderSize = {};

                //テーブルヘッダの要素を操作して保存する必要があったら
                $(arg.tableElementID + " th").each(function (index, element) {

                    //テーブルサイズの保存指定が指定があったら
                    //そのテーブルに対して、横幅を保存する
                    var saveInfo = $(element).attr("save-column-width-size-name");
                    if (null != saveInfo) {
                        var w = $(element).width();
                        tableHeaderSize[saveInfo] = w;
                        //console.log(w);
                    }
                });

                //ブラウザローカルDBに保存する
                (function () {
                    var tStoreID = LF_getStoreID(arg.tableElementID);
                    var now = store.get(tStoreID);
                    if (null != now) {
                        //既存の値が有ったら消去する
                        store.remove(tStoreID);
                    }

                    //横幅の値を保存する
                    store.set(tStoreID, tableHeaderSize);
                })();
            });
        },
        /**
         * ソート用にテーブルヘッダがクリックされたからソート処理に飛ばす。
    
                var arg = {
                    //ソート対象となる列名はhiddenフィールドに入れてサーバサイドに送り、
                    //サーバー側で処理させる。
                    //ソート対象の列名が入るinput要素名
                    sortOrderName: "",
                    //post対象となるフォーム名
                    formName : "",
                    //ソートするカラム名
                    columnName:"",
                    //ソート順序 : ASC ot DES
                    sortOrder:""
                };
         */
        onTableHeaderClickForSort: function (arg) {

            //ローディング表示を行う
            VegaUtility.StaticFunctions.LoadingSplash.show();

            //カラムが同じだったら、ソート列を入れ替える
            (function () {

                var now = $(arg.sortOrderName).val();
                if (now === arg.columnName) {
                    //ソート条件を反転させる
                    var order = $(arg.sortOrder).val();

                    if ("ASC" == order) {
                        $(arg.sortOrder).val("DES");
                    }
                    else {
                        $(arg.sortOrder).val("ASC");
                    }

                }

            })();

            //ソート順序はsortOrder要素にに持たせる。
            //この要素がクリックされたらソートオーダー列を書き換える
            $(arg.sortOrderName).val(arg.columnName);

            //サーバー側に同期して再ソートさせる
            var tForm = $(arg.formName);
            tForm.submit();
        }
    }


// -- -- * -- -- *  -- -- *  -- -- 
//ページネーション関係の処理を実装する
VegaUtility.StaticFunctions.PaginationHelper =
    {
        //指定されたページに移動する
        movePage: function (idName, index) {

            //ローディング表示を行う
            VegaUtility.StaticFunctions.LoadingSplash.show();

            //遷移先ページをhidden要素に指定する
            $("#" + idName).val(index);

            var p = $("#" + idName).parents('form');

            //親となるフォーム要素を探す
            var tForm = $("#" + idName).parents('form');
            tForm.submit();
        }
    }


// -- -- * -- -- *  -- -- *  -- -- 
//サーバー時間表示系の処理を更新
VegaUtility.StaticFunctions.ServerTime =
    {
        //画面上の時間をサーバーで描画された時間で更新する
        update: function (intervalMillisecond) {

            //更新済みの現在時間を取得する
            var updatedNowTime = (function () {

                //データが保管されている要素
                var srcElement = $("#server-time-on-when-page-rendered-src");

                //サーバー時間の秒数を探す
                var srcTime = srcElement.text();

                //dateオブジェクトに変換する
                var dSrc = luxon.DateTime.fromISO(srcTime);

                //時間を進める
                var now = dSrc.plus({ milliseconds: intervalMillisecond });

                //現在の値を更新する
                srcElement.text(now.toISOTime());

                return now;
            })();

            //画面上に表示する時間を更新する
            (function () {

                //更新時間
                var timeStr = updatedNowTime.toFormat('yyyy/L/d(ccc) H:m');

                //該当要素を探して更新する
                $('#server-time-on-when-page-rendered-view').text(timeStr);

            })();
        }
    }

// -- -- * -- -- *  -- -- *  -- -- 
//確認ダイアログ
VegaUtility.StaticFunctions.ConfirmationDialog =
    {
    //確認ダイアログの表示が必要か判定する
    //
    //[戻り値]
    //true : 確認表示が必要
    //false : 確認表示が不要
    isNeedShow: function(){

        var ele = $('#is-need-not-user-confirm-dialog-flag');
        if (null != ele) {
            //ダイアログボックスが表示不要だったら、
            //trueが立ってくるで、確認する
            return !VegaUtility.StaticFunctions.Helpers.parseBoolean(ele.text());
        }
        return true;
    }
}

// -- -- * -- -- *  -- -- *  -- -- 
//数学
VegaUtility.StaticFunctions.Math =
    {
        // 数値チェック
        //
        //[戻り値]
        //true : 数字だけで構成されている
        //false : 数字以外が混じっている
        isNumber: function (num) {
            let regex = new RegExp(/^[0-9]+$/);
            if (false == regex.test(num)) {
                return false;
            }

            return true;
        },

        // 数値をカンマ区切り表記にする
        // https://qiita.com/ariyo13/items/ab410a84c74b23099648
        commaSplit: function (num) {
            return num.toString().replace(/(\d)(?=(\d{3})+$)/g, '$1,');
        }
    }