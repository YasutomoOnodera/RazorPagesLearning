using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.Pagination
{

    /// <summary>
    /// ページネーション情報
    /// </summary>
    public class PaginationInfo
    {
        public PaginationInfo()
        {
            ///表示件数を更新する
            setDisplayNumberItems(new List<int> { 20, 50, 100 });

        }

        public static PaginationInfo createInstance(int maxItems)
        {
            var funcRet = new PaginationInfo();

            funcRet.maxItems = maxItems;

            //ページを移動する
            funcRet.movePage(0);

            return funcRet;
        }

        /// <summary>
        /// 最大表示件数
        /// </summary>
        public int maxItems { get; set; }

        /// <summary>
        ///次に表示するべきページ
        /// </summary>
        public int displayNextPage { get; set; }

        /// <summary>
        /// 現在のページで表示されているレコードの開始件数番号
        /// </summary>
        public int startViewItemIndex { get; set; }

        /// <summary>
        /// 現在のページで表示されているレコードの終了件数
        /// </summary>
        public int endViewItemIndex { get; set; }


        /// <summary>
        /// データ表示件数
        /// </summary>
        /// 
        [BindProperty]
        public string displayNumber { get; set; }

        /// <summary>
        /// データ表示項目
        /// </summary>
        public List<SelectListItem> displayNumbers { get; set; }

        /// <summary>
        /// 表示項目を更新する
        /// </summary>
        /// <param name="num"></param>
        public void setDisplayNumberItems(List<int> num)
        {
            this.displayNumbers = num.Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.ToString()
            }).ToList();
            this.displayNumber = num[0].ToString();
        }

        /// <summary>
        /// 現在のページを選択
        /// </summary>
        [BindProperty]
        public bool checkThisPage { get; set; }

        /// <summary>
        /// 全ページを選択
        /// </summary>
        [BindProperty]
        public bool checkAllPage { get; set; }

        /// <summary>
        /// 現在のページネーションページ
        /// </summary>
        [BindProperty]
        public int nowPaginationPage { get; set; }

        /// <summary>
        /// 最大ページ数を算出する
        /// </summary>
        /// <returns></returns>
        public int getMaxPages()
        {
            var disp = int.Parse(this.displayNumber);
            var r = this.maxItems / disp;
            if (0 != this.maxItems % disp)
            {
                //端数を補正する
                r = r + 1;
            }
            return r;
        }

        /// <summary>
        /// ページネーションで表示されるページ情報
        /// </summary>
        private int PaginationDisplayPages = 4;

        /// <summary>
        /// 指定されたページがページネーションで表示対象となるページ
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool addListInDisplayRange(List<int> list, int index)
        {
            if (index > getMaxPages())
            {
                return false;
            }
            if (0 > index)
            {
                return false;
            }

            //ページネーション対象に入っていたら、リストに投入する
            list.Add(index);

            return true;
        }

        /// <summary>
        /// ページネーションとして表示する近隣ページ一覧
        /// </summary>
        /// <returns></returns>
        public List<int> getNeighborPages()
        {
            if (PaginationDisplayPages > getMaxPages())
            {
                //ページネーションに必要な分だけページが無かったら、
                //現在ある分だけで表示する
                return Enumerable.Range(1, getMaxPages()).ToList();
            }
            else if (PaginationDisplayPages > this.nowPaginationPage)
            {
                //ページネーションに必要な分だけページが無かったら、
                //現在ある分だけで表示する
                return Enumerable.Range(1, PaginationDisplayPages).ToList();
            }
            else
            {
                //ページネーションに必要な分だけページが存在する
                var rr = new List<int>();
                addListInDisplayRange(rr, this.nowPaginationPage - 2);
                addListInDisplayRange(rr, this.nowPaginationPage - 1);

                addListInDisplayRange(rr, this.nowPaginationPage + 1);
                addListInDisplayRange(rr, this.nowPaginationPage + 2);
                return rr;
            }
        }

        /// <summary>
        /// ページネーション情報を移動させる
        /// 先に maxItems の情報を設定する必要あり
        /// </summary>
        /// <param name="index"></param>
        public void movePage(int index)
        {
            #region 条件チェック
            if (0 > index)
            {
                //マイナスページは何もしない
                return;
            }
            if (index > getMaxPages())
            {
                //ページ数が正しくないので何もしない
                return;
            }
            #endregion

            //計算する
            {
                var dn = int.Parse(this.displayNumber);
                this.nowPaginationPage = index;
                //表示上は1ページ起源とするため
                if (1 < this.nowPaginationPage)
                {
                    this.startViewItemIndex = (this.nowPaginationPage - 1) * dn;
                }
                else
                {
                    this.startViewItemIndex = 0;
                }
                this.endViewItemIndex = this.startViewItemIndex + dn;
            }
        }

        /// <summary>
        /// 次のページを取得する
        /// </summary>
        /// <returns></returns>
        public int getNextPage()
        {
            var np = this.nowPaginationPage + 1;
            if (np > this.getMaxPages())
            {
                np = this.getMaxPages();
            }
            return np;
        }

        /// <summary>
        /// 前のページを取得する
        /// </summary>
        /// <returns></returns>
        public int getPreviousPage()
        {
            var np = this.nowPaginationPage - 1;
            if (0 > np)
            {
                np = 0;
            }
            return np;
        }
    }

}
