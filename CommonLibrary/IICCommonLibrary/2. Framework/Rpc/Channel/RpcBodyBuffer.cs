/*
 * RpcBodyBuffer
 * 
 * RpcBody的包装类，为了将Serialize/Deserialize从Channel层提到，Rpc的Framework层，而增设
 * 
 * 该Class有以下状态
 *		Request:	
 *				Out：
 *					null,
 *					<T>
 *					
 *					ToStream();
 *					ToByteArray();
 *					
 *					GetObject() for tracing
 *				In:
 *					FromStream
 *					FromByteArray
 *					
 *					Read<T>		: lazy
 *					GetObject() for tracing
 *					
 *		Resoponse
 *				Out:
 *					null,
 *					<T>
 *					Exception
 *					
 *					ToStream();
 *					ToByteArray();
 *					
 *					GetObject() for tracing	
 *					
 *				In:
 *					FromStream
 *					FromByteArray
 *					
 *					Read<T>
 *					ReadException()
 *					
 *					GetObject()
 *					
 * 
 * GaoLei
 * 2010-06-11
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public enum RpcBodyBufferMode
	{
		DataOutput,
		ErrorOutput,
		Input,
	};

	public class RpcBodyBuffer
	{
		private object _value;
		private byte[] _buffer;
		private bool _textError;
		private RpcBodyBufferMode _mode;

		public object Value
		{
			get { return _value; }
		}

		public RpcBodyBufferMode Mode
		{
			get { return _mode; }
		}

		public bool TextError
		{
			get { return _textError; }
			set { _textError = value; }
		}

		public RpcBodyBuffer(Exception ex)
			: this(RpcBodyBufferMode.ErrorOutput, ex)
		{
		}

		protected RpcBodyBuffer(RpcBodyBufferMode mode, object value)
		{
			_value = value;
			_mode = mode;
		}

		public RpcBodyBuffer(Stream stream, int len)
		{
			_value = null;
			_mode = RpcBodyBufferMode.Input;
			_buffer = new byte[len];

            int a = 0;
            int r = 0;
            while (a < len) {
                r = stream.Read(_buffer, a, len - a);
                if (r <= 0)
                    throw new Exception("input stream not enought");
                a += r;
            }
		}

		public RpcBodyBuffer(byte[] buffer)
		{
			_value = null;
			_mode = RpcBodyBufferMode.Input;
			_buffer = buffer;
		}

		public virtual int GetSize()
		{
			switch (_mode) {
				case RpcBodyBufferMode.ErrorOutput:
				case RpcBodyBufferMode.Input:
					if (_buffer == null) {
						SerializeError();
					}
					return _buffer.Length;
				case RpcBodyBufferMode.DataOutput:
					throw new NotSupportedException("not in root Buffer");
				default:
					throw new NotSupportedException("unexcepted:" + _mode);
			}
		}

		private void SerializeError()
		{
			if (_textError) {
				_buffer = Encoding.UTF8.GetBytes(_value.ToString());
			} else {
				_buffer = BinarySerializer.ToByteArray(_value);
			}
		}

		public virtual int WriteToStream(Stream stream)
		{
			switch (_mode) {
				case RpcBodyBufferMode.ErrorOutput:
					if (_buffer == null) {
						SerializeError();
					}
					stream.Write(_buffer, 0, _buffer.Length);
					return _buffer.Length;
				case RpcBodyBufferMode.Input:
					stream.Write(_buffer, 0, _buffer.Length);
					return _buffer.Length;
				default:
					throw new NotSupportedException("Mode Not Support:" + _mode);
			}
		}

		public virtual byte[] GetByteArray()
		{
			switch (_mode) {
				case RpcBodyBufferMode.ErrorOutput:
					if (_buffer == null) {
						SerializeError();
					}
					return _buffer;
				case RpcBodyBufferMode.Input:
					return _buffer;
				default:
					if (_buffer != null)
						return EmptyBuffer;
					else
						throw new NotSupportedException("Mode Not Support:" + _mode);
			}
		}

		public virtual V GetValue<V>()
		{
			if (_value != null)
				return (V)_value;

			switch (_mode) {
				case RpcBodyBufferMode.Input:
					_value = ProtoBufSerializer.FromByteArray<V>(_buffer);
					return (V)_value;
				case RpcBodyBufferMode.DataOutput:
					return (V)_value;
				default:
					throw new NotSupportedException("Mode Not Support:" + _mode);
			}
		}

		public virtual Exception GetException()
		{
			if (_value != null)
				return (Exception)_value;

			switch (_mode) {
				case RpcBodyBufferMode.Input:
					if (_textError) {
						_value = new Exception(Encoding.UTF8.GetString(_buffer));
					} else {
						_value = BinarySerializer.FromByteArray<Exception>(_buffer);
					}
					return (Exception)_value;
				case RpcBodyBufferMode.ErrorOutput:
					return (Exception)_value;
				default:
					throw new NotSupportedException("Mode Not Support:" + _mode);	
			}
		}
		
		public static byte[] EmptyBuffer = new byte[0];
		public static RpcBodyBuffer EmptyBody = new RpcBodyBufferEmpty();
	}

	public sealed class RpcBodyBuffer<T> : RpcBodyBuffer
	{
		private T _v;
		private int _size = -1;

		public RpcBodyBuffer(T value)
			: base(RpcBodyBufferMode.DataOutput, value)
		{
			_v = value;
		}

		public override int GetSize()
		{
			if (_size < 0) {
				_size = (int)ProtoBufSerializer.GetSize(_v);
			}
			return _size;
		}

		public override int WriteToStream(Stream stream)
		{
			if ((object)_v != null) {
				return (int)ProtoBufSerializer.Serialize<T>(stream, _v);
			} else {
				return 0;
			}
		}

		public override byte[] GetByteArray()
		{
			return ProtoBufSerializer.ToByteArray<T>(_v);
		}
	}

	public sealed class RpcBodyBufferEmpty : RpcBodyBuffer
	{
		private MemoryStream _emptyStream;

		public RpcBodyBufferEmpty()
			: base(new byte[0])
		{
			_emptyStream = new MemoryStream(new byte[0], false);
		}

		public override V GetValue<V>()
		{
			return ProtoBufSerializer.Deserialize<V>(_emptyStream);
		}

		public override Exception GetException()
		{
			return null;
		}
	}
}
