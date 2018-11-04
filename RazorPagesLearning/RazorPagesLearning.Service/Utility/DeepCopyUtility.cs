using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace RazorPagesLearningL.Service.Utility
{
    public static class DeepCopyUtility
    {
        //--------------------------------------------------------------------------------
        // 引数に渡したオブジェクトをディープコピーしたオブジェクトを生成して返す
        // ジェネリックメソッド版
        //--------------------------------------------------------------------------------
        internal static T DeepCopy<T>(this T target)
        {

            T result;
            BinaryFormatter b = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();

            try
            {
                b.Serialize(mem, target);
                mem.Position = 0;
                result = (T)b.Deserialize(mem);
            }
            finally
            {
                mem.Close();
            }

            return result;
        }

    }
}
