// Copyright© 2016 smdn.Released under the MIT License.
// https://smdn.jp/misc/about/#smdn
// https://smdn.jp/misc/license/MIT/

#define ENABLE_KEYS_VALUES
//#undef ENABLE_KEYS_VALUES

using System;
using System.Linq;
using System.Collections.Generic;

namespace RazorPagesLearning.Service.Utility.Generic
{
	/// <summary>
	/// ジェネリック版のOrderedDictionary実装。
	/// </summary>
	/// <remarks>
	///  <para>
	///   System.Collections.ObjectModel.Collection&lt;KeyValuePair&lt;TKey, TValue&gt;&gt;をベースとして構築したOrderedDictionary。
	///  </para>
	///  <para>
	///   このクラスにおける制限事項等は次の通り。
	///   <list type="bullet">
	///    <item>
	///     <description>
	///      インデクサthis[int index]の型はTValueではなく、KeyValuePair&lt;TKey, TValue&gt;。
	///    　つまりインデックスによるアクセスでは値を取得・設定するのではなく、KeyValuePairを取得・設定することになる。
	///     </description>
	///    </item>
	///    <item><description>キーによるアクセスはO(n)、インデックスによるアクセスはO(1)となる。</description></item>
	///    <item><description>容量(Capacity)を指定することはできない。</description></item>
	///    <item><description>ISerializableを実装していない。　シリアライズの動作が未定義。</description></item>
	///    <item>
	///     <description>
	///      Keysプロパティ・Valuesプロパティは、アクセス時にキャッシュされたコレクションを別途生成する。
	///      このプロパティの実装が不要な場合はENABLE_KEYS_VALUESをundefにすることで無効化できる。
	///     </description>
	///    </item>
	///   </list>
	///  </para>
	/// </remarks>
	public class OrderedDictionary<TKey, TValue> :
	  System.Collections.ObjectModel.Collection<KeyValuePair<TKey, TValue>>,
	  IDictionary<TKey, TValue>
	{
		private readonly IEqualityComparer<TKey> keyComparer;

		public OrderedDictionary()
		  : this(EqualityComparer<TKey>.Default)
		{
		}

		public OrderedDictionary(IEqualityComparer<TKey> keyComparer)
		{
			this.keyComparer = keyComparer;
		}

		public void Add(TKey key, TValue val)
		{
			Add(new KeyValuePair<TKey, TValue>(key, val));
		}

		public void Insert(int index, TKey key, TValue val)
		{
			Insert(index, new KeyValuePair<TKey, TValue>(key, val));
		}

		public bool Remove(TKey key)
		{
			int index;

			if (TryGetIndex(key, out index))
			{
				RemoveAt(index);

				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ContainsKey(TKey key)
		{
			int index;

			return TryGetIndex(key, out index);
		}

		public bool TryGetValue(TKey key, out TValue val)
		{
			int index;

			if (TryGetIndex(key, out index))
			{
				val = this[index].Value;

				return true;
			}
			else
			{
				val = default(TValue);

				return false;
			}
		}

		/// <summary>
		/// キーに対応するインデックスを取得する。
		/// </summary>
		/// <returns>キーに該当する要素がある場合は<c>true</c>、ない場合は<c>false</c>。</returns>
		/// <param name="key">インデックスを取得したい要素のキー。</param>
		/// <param name="index">キーに該当する要素がある場合、そのインデックス。</param>
		private bool TryGetIndex(TKey key, out int index)
		{
			for (index = 0; index < Count; index++)
			{
				if (keyComparer.Equals(this[index].Key, key))
					return true;
			}

			return false;
		}

		public TValue this[TKey key]
		{
			get
			{
				int index;

				if (TryGetIndex(key, out index))
					return this[index].Value;
				else
					throw new KeyNotFoundException(string.Format("item not found; key = {0}", key));
			}
			set
			{
				int index;

				if (TryGetIndex(key, out index))
					this[index] = new KeyValuePair<TKey, TValue>(key, value);
				else
					Add(key, value);
			}
		}

#if ENABLE_KEYS_VALUES
		private ICollection<TKey> keys = null;
		private ICollection<TValue> values = null;

		private bool modified = true;

		/// <summary>
		/// キャッシュされたkeys, valuesを最新の状態にする。
		/// </summary>
		/// <remarks>
		/// 前回のキャッシュ生成以降にコレクションが変更されていれば、キャッシュを破棄して生成しなおす。
		/// </remarks>
		private void EnsureKeysAndValuesUpdated()
		{
			if (!modified)
				return;

			keys = this.Select(pair => pair.Key).ToList().AsReadOnly();
			values = this.Select(pair => pair.Value).ToList().AsReadOnly();

			modified = false;
		}
#endif

		public ICollection<TKey> Keys
		{
			get
			{
#if ENABLE_KEYS_VALUES
				EnsureKeysAndValuesUpdated();

				return keys;
#else
      throw new NotSupportedException();
#endif
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
#if ENABLE_KEYS_VALUES
				EnsureKeysAndValuesUpdated();

				return values;
#else
      throw new NotSupportedException();
#endif
			}
		}

		protected override void InsertItem(int index, KeyValuePair<TKey, TValue> item)
		{
			int existentIndex;

			if (TryGetIndex(item.Key, out existentIndex))
				throw new ArgumentException(string.Format("the item already exists; key = {0}", item.Key));

			base.InsertItem(index, item);

#if ENABLE_KEYS_VALUES
			modified = true;
#endif
		}

		protected override void SetItem(int index, KeyValuePair<TKey, TValue> item)
		{
			int existentIndex;

			if (TryGetIndex(item.Key, out existentIndex) && index != existentIndex)
				throw new ArgumentException(string.Format("the item already exists; key = {0}", item.Key));

			base.SetItem(index, item);

#if ENABLE_KEYS_VALUES
			modified = true;
#endif
		}

#if ENABLE_KEYS_VALUES
		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);

			modified = true;
		}

		protected override void ClearItems()
		{
			base.ClearItems();

			modified = true;
		}
#endif
	}
}
