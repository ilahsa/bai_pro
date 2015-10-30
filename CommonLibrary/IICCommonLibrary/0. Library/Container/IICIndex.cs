/*
 * A Powerful Index builder
 * 
 * Author:	Lei Gao 
 * Created:	2008-04-07
 */ 
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class IICIndex<T>
	{
		private FieldInfo[] _fields;
		private SortedList<IICIndexObject, T> _indexEntrys = new SortedList<IICIndexObject,T>();
		private IICReaderWriterLock _innerLock = new IICReaderWriterLock();

		public IICIndex(params string[] fieldNames)
			: this(null, fieldNames)
		{

		}

		public IICIndex(IEnumerable<T> items, params string[] fieldNames)
		{
			_fields = new FieldInfo[fieldNames.Length];
			for (int i = 0; i < fieldNames.Length; i++) {
				string fieldName = fieldNames[i];
				_fields[i] = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (_fields[i] == null) {
					string msg = string.Format("Fields Not Found: {0}.{1}", typeof(T).Name, fieldName);
					throw new KeyNotFoundException(msg);
				}
			}
			if (items != null) {
				BuildIndex(items);
			}
		}

		public IList<T> Find(params IComparable[] startFields)
		{
			if (startFields.Length > _fields.Length) 
				throw new InvalidOperationException("Too many search field");

			IComparable[] values = new IComparable[_fields.Length];
			for (int i = 0; i < startFields.Length; i++) {
				values[i] = startFields[i];
			}
						
			List<T> list = new List<T>();
            if (_indexEntrys.Count == 0)
                return list;

			IICIndexObject startObject = new IICIndexObject(values, 0);

			using (IICLockRegion region = _innerLock.LockForRead()) {
				int start = SearchFirstItem(startObject);
				if (start < 0)
					return list;
				
				if (!_indexEntrys.Keys[start].StartsWith(startObject))
					start++;
				
				for (int i = start; i < _indexEntrys.Count && _indexEntrys.Keys[i].StartsWith(startObject); i++) {
					list.Add(_indexEntrys.Values[i]);
				}
			}
			return list;
		}

		public T FindOne(params IComparable[] startFields)
		{
			T ret = TryFindOne(startFields);
			if (ret.Equals(default(T))) {
				StringBuilder str = new StringBuilder();
				foreach (string field in startFields) {
					str.Append(field);
					str.Append(",");
				}
				throw new KeyNotFoundException(string.Format("Index<{0}> Not Found: {1}", typeof(T).Name, str));
			} else {
				return ret;
			}
		}

		public T TryFindOne(params IComparable[] startFields)
		{
			IList<T> rets = Find(startFields);
			if (rets.Count == 0) {
				return default(T);
			}

			if (rets.Count > 1) {
				throw new Exception(string.Format("Index<{0}> Return More: {1} Count: {2}", typeof(T).Name, startFields[0], rets.Count > 1));
			}

			return rets[0];
		}

		public T FindFirst(params IComparable[] startFields)
		{
			IList<T> rets = Find(startFields);
			if (rets.Count == 0) {
				return default(T);
			}

			if (rets.Count > 1) {
				throw new Exception(string.Format("Index<{0}> Return More: {1} Count: {2}", typeof(T).Name, startFields[0], rets.Count > 1));
			}

			return rets[0];
		}

        public IList<T> FindKeys(int keyFieldCount, params IComparable[] startFields)
        {
            return FindKeys(keyFieldCount, null, startFields);
        }

        public IList<T> FindKeys(int keyFieldCount, Func<T, bool> judgement, params IComparable[] startFields)
        {
            if (startFields.Length > _fields.Length)
                throw new InvalidOperationException("Too many search fields: IICIndex<" + typeof(T).Name + ">");

            if (keyFieldCount > _fields.Length)
                throw new InvalidOperationException("Too many key fields: IICIndex<" + typeof(T).Name + "> key:" + keyFieldCount);

            if (keyFieldCount < startFields.Length)
                throw new InvalidOperationException("KeyFieldCount can not less than startFields: IICIndex<" + typeof(T).Name + "> key:" + keyFieldCount);

            List<T> list = new List<T>();
            if (_indexEntrys.Count == 0)
                return list;

            IComparable[] values = new IComparable[_fields.Length];
            for (int i = 0; i < startFields.Length; i++)
            {
                values[i] = startFields[i];
            }
            IICIndexObject startObject = new IICIndexObject(values, 0);
            IICIndexObject lastObject = null;

            using (IICLockRegion region = _innerLock.LockForRead())
            {
                for (int i = 0; i < _indexEntrys.Count; i++)
                {
                    IICIndexObject keyObjects = _indexEntrys.Keys[i];
                    if (keyObjects.StartsWith(startObject))
                    {
                        if (lastObject == null || !keyObjects.StartsWith(lastObject, keyFieldCount))
                        {
                            if (judgement != null && judgement(_indexEntrys.Values[i]))
                            {
                                list.Add(_indexEntrys.Values[i]);
                                lastObject = keyObjects;
                            }
                        }
                    }
                }
            }
            return list;
        }
        /*
		public IList<T> FindKeys(int keyFieldCount, params IComparable[] startFields)
		{
			if (startFields.Length > _fields.Length)
				throw new InvalidOperationException("Too many search fields: IICIndex<" +  typeof(T).Name + ">");

			if (keyFieldCount > _fields.Length)
				throw new InvalidOperationException("Too many key fields: IICIndex<" + typeof(T).Name + "> key:" + keyFieldCount);

			if (keyFieldCount < startFields.Length)
				throw new InvalidOperationException("KeyFieldCount can not less than startFields: IICIndex<" + typeof(T).Name + "> key:" + keyFieldCount);

			List<T> list = new List<T>();
			if (_indexEntrys.Count == 0)
				return list;

			IComparable[] values = new IComparable[_fields.Length];
			for (int i = 0; i < startFields.Length; i++) {
				values[i] = startFields[i];
			}

			IICIndexObject startObject = new IICIndexObject(values, 0);
			IICIndexObject lastObject = null;

			using (IICLockRegion region = _innerLock.LockForRead()) {
				int start = SearchFirstItem(startObject);
				if (start < 0)
					return list;

				if (!_indexEntrys.Keys[start].StartsWith(startObject))
					start++;

				for (int i = start; i < _indexEntrys.Count && _indexEntrys.Keys[i].StartsWith(startObject); i++) {
					IICIndexObject keyObjects = _indexEntrys.Keys[i];
					if (keyObjects.StartsWith(lastObject)) {
					}
			
					list.Add(_indexEntrys.Values[i]);
				}
			}
			return list;
		}
        */

		public void BuildIndex(IEnumerable<T> items)
		{
			int serial = 0;
			using (IICLockRegion region = _innerLock.LockForWrite()) {
				_indexEntrys.Clear();
				foreach (T item in items) {
					IComparable[] vals = new IComparable[_fields.Length];
					for (int i = 0; i < _fields.Length; i++) {
						vals[i] = (IComparable)_fields[i].GetValue(item);
					}
					IICIndexObject indexObj = new IICIndexObject(vals, 0);
					if (_indexEntrys.ContainsKey(indexObj)) {
						serial++;
						indexObj.SerialNo = serial;
					}
					_indexEntrys.Add(indexObj, item);
				}
			}
		}

		private int SearchFirstItem(IICIndexObject indexObj)
		{
			int index;
			int begin = 0;
			int end = _indexEntrys.Count - 1;

			IICIndexObject obj;
			while (end - begin > 1) {
				int middle = (end + begin) / 2;
				obj = _indexEntrys.Keys[middle];

				int compareResult = indexObj.CompareTo(obj);
				if (compareResult == 0) {
					index = middle;
					return index;
				} else {
					if (compareResult > 0) {
						begin = middle;
					} else {
						end = middle;
					}
				}
			}

			if (end == begin) {
				if (indexObj.CompareTo(_indexEntrys.Keys[begin]) >= 0)
					return begin;
			} else {
				if (indexObj.CompareTo(_indexEntrys.Keys[begin]) >= 0)
					return begin;

				if (indexObj.CompareTo(_indexEntrys.Keys[end]) >= 0) {
					return end;
				}
			}
			return 0;
		}
	}

	class IICIndexObject: IComparable
	{
		private IComparable[] _objects;
		private int _serialNo;

		public IICIndexObject(IComparable[] objects, int serialNo) 
		{
			_objects = objects;
			_serialNo = serialNo;
		}
		
		public int SerialNo
		{
			get { return _serialNo; }
			set { _serialNo = value; }
		}
		#region IComparable Members

		public int CompareTo(object obj)
		{
			IICIndexObject val = obj as IICIndexObject;
			if (obj == null)
				throw new InvalidCastException("Can't Compare IICIndexObject With: " + obj.GetType());

 			for (int i = 0; i < _objects.Length; i++) {
				if (this._objects[i] == null) {
					if (val._objects[i] == null)
						continue;
					else
						return -1;
				} else {
					if (val._objects[i] == null)
						return 1;
					else {
						int c = this._objects[i].CompareTo(val._objects[i]);
						if (c == 0)
							continue;
						else
							return c;
					}	
				}
			}
			return this._serialNo - val._serialNo;
		}
		#endregion

        public bool StartsWith(IICIndexObject prefix)
        {
            return StartsWith(prefix, _objects.Length);
        }

        public bool StartsWith(IICIndexObject prefix, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (prefix._objects[i] != null)
                {
                    if (_objects[i].CompareTo(prefix._objects[i]) == 0)
                        continue;
                    else
                        return false;
                }
                else
                {
                    break;
                }
            }
            return true;
        }
        /*
		public bool StartsWith(IICIndexObject prefix)
		{
			for (int i = 0; i < _objects.Length; i++) {
				if (prefix._objects[i] != null) {
					if (_objects[i].CompareTo(prefix._objects[i]) == 0)
						continue;
					else
						return false;
				} else {
					break;
				}
			}
			return true;
		}*/

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			foreach (object obj in _objects) {
				str.AppendFormat("{0} ", obj);
			}
			return str.ToString();
		}
	}
}

/*
'JAPH V1.0';$/=0xE0;$^F=1<<5;$;=$^F>>3;$,=
'Nywx$           /\/\       x7Fy~'.
'Ersxl      ,"~~~    \      kde~b'.
'iv$Ti     /        @ \_    ox*Zo'.
'vp$Le  ~~|           __0   f*Bki'.
'goiv2     \||||--||-|/     ax0ox';
{$_=chr(ord(substr($,,$"++,1))-$;);print;
$"=($"&$/)+$^Fif$"%$^F>$;;redo if$"<=$^F*$;+$;;}
*/