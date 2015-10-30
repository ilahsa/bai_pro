using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class DummyStream: Stream
	{
		private long position;
		private long size;

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get { return long.MaxValue; }
		}

		public override long Position
		{
			get	{ return position; }
			set { position = value; }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("DummyStream not support read()");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin) {
				case SeekOrigin.Begin:
					position = offset;
					break;
				case SeekOrigin.End:
					position = size + offset;
					break;
				case SeekOrigin.Current:
					position += offset;
					break;
			}
			return position;
		}

		public override void SetLength(long value)
		{
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			position = position + count;
			if (size < position)
				size = position;
		}
	}
}
