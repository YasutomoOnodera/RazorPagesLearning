using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB.Helper
{
    /// <summary>
    /// entity framework においてクエリを書くための補助的な関数
    /// </summary>
    public static class QueryHelper
    {
        /// <summary>
        /// Entity Frameworkにおして、orで動的に複数条件を結合する場合、
        /// C#の式木を動的に生成する必要がある。
        /// この関数では、指定されたカラムに対して、
        /// 文字列で複数のキーワードでor検索指定する式目を生成する。
        /// 
        /// [参考]
        /// Expressionを使った動的なOR文の生成
        /// http://d.hatena.ne.jp/coma2n/20080717/1216269202
        /// ↑
        /// ここのコードを参考に改造している。
        /// 
        /// その他、参考になりそうな情報
        /// 
        /// LINQ文で動的にWhere句を組み立てるには？［3.5、C#、VB］
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/986dynamiclinq/dynamiclinq.html
        /// 
        /// LINQ で動的に OR 検索するやつ
        /// https://tech.blog.aerie.jp/entry/2015/09/25/103325
        /// 
        /// 式木（Expression Trees）
        /// https://ufcpp.net/study/csharp/sp3_expression.html
        /// 
        /// </summary>
        /// <typeparam name="TableType"></typeparam>
        /// <param name="keywords"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static System.Linq.Expressions.Expression<Func<TableType, bool>>
            makeQueryOfKeywordJoinedByOr<TableType>( IEnumerable<string> keywords , string columnName )
        {
            if (0 == keywords.Count())
            {
                throw new ArgumentException("キーワードが指定されていません。");
            }

            // string.Containsメソッド
            var contains = typeof(string).GetMethod("Contains");
            // ラムダ式に渡すパラメータ
            var paramExpr = Expression.Parameter(typeof(TableType), "d");

            Expression bodyExpr = null;
            foreach (var o in keywords)
            {
                if (o.Length == 0) continue;

                if (bodyExpr == null)
                {
                    // d.FileName.Contains("値")のコードと等価
                    bodyExpr = Expression.Call(
                        Expression.Property(paramExpr, columnName),
                        contains, Expression.Constant(o)
                    );

                }
                else
                {
                    // 既に式があればOR演算する
                    bodyExpr = Expression.OrElse(
                        bodyExpr,
                        Expression.Call(
                            Expression.Property(paramExpr, columnName),
                            contains, Expression.Constant(o)
                        )
                    );
                }
            }
            return Expression.Lambda<Func<TableType, bool>>(bodyExpr, paramExpr);
        }
    }
}
