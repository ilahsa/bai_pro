//*
// * 彪悍的MemoryStream，与SocketAsyncEventArgs一起使用，可以发挥最大效力 
// * 相比MemoryStream，不会在扩容的时候进行内存Realloc，面对未知大小的Serialize的时候有很高的性能
// * 适合进行大对象的ProtocolBuffer的序列化
// *  
// * Gao Lei 
// * 2010-06-24
// */ 
//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.Text;

//namespace Imps.Services.CommonV4
//{
//    public sealed class RpcBufferStream: Stream
//    {
//        private byte[] _firstbuffer;
//        private byte[] _secondBuffer;

//        private int _offset;
//        private int _count;
//        private int _capacity;

//        public RpcBufferStream(byte[] firstBuffer)
//        {
//            _firstbuffer = firstBuffer;
//            _secondBuffer = null;

//            _offset = 0;
//            _count = 0;
//            _capacity = firstBuffer.Length;
//        }

//        public int SecondOffset
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public int SecondCount
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override bool CanRead
//        {
//            get { return true; }
//        }

//        public override bool CanSeek
//        {
//            get { return true; }
//        }

//        public override bool CanWrite
//        {
//            get { return true; }
//        }

//        public override void Flush()
//        {	
//        }

//        public override long Length
//        {
//            get { return _count; }
//        }

//        public override long Position
//        {
//            get	{ return _offset; }
//        }

//        public override int Read(byte[] buffer, int offset, int count)
//        {
//            int readBytes = 0;
//            while (count > 0) {
//                if (_currentCount - _currentOffset >= count) {
//                    for (int i = 0; i < count; i++) {
//                        buffer[offset + i] = _currentBuffer[_currentOffset + i];
//                    }
//                    readBytes += count;
//                    break;
//                } else {
//                    int read = _currentCount - _currentOffset;
					
//                    for (int i = 0; i < read; i++) {
//                        buffer[offset + i] = _currentBuffer[_currentOffset + i];
//                    }

//                    offset += read;
//                    count -= read;
//                    _currentBufferIndex++;
//                    if (_bufferList != null && _currentBufferIndex >= _bufferList.Count) {
//                        var nextBuffer = _bufferList[_currentBufferIndex - 1];
//                        _currentBuffer = nextBuffer.Array;
//                        _currentOffset = 0;
//                        _currentCount = nextBuffer.Count;
//                    } else {

//                    }
//                }
//            }

//            _offset += count;
//            return readBytes;
//        }

//        public override long Seek(long offset, SeekOrigin origin)
//        {
//            switch (origin) {
//                case SeekOrigin.Begin:
//                    _offset = (int)offset;
//                    if (_offset < _firstbuffer.Length) {
//                        _currentBuffer = _firstbuffer;
//                        _currentBufferIndex = 0;
//                        _currentOffset = _offset;
//                        _currentCount = _count > _firstbuffer.Length ? _firstbuffer.Length : _count;
//                    } else {
						
//                    }
//                    break;
//                case SeekOrigin.Current:
//                    _offset = (int)offset;

//                    break;
//                case SeekOrigin.End:
//                    break;
//            }
//        }

//        public override void SetLength(long value)
//        {
			
//        }
//        public override void Write(byte[] buffer, int offset, int count)
//        {
//            if (_currentBuffer.Length - _currentOffset) {

//            }
//        }

//        public string ReadString(BinaryReader reader)
//        {

//        }

//        public void Write(int value)
//        {
//            this._buffer[0] = (byte) value;
//            this._buffer[1] = (byte) (value >> 8);
//            this._buffer[2] = (byte) (value >> 0x10);
//            this._buffer[3] = (byte) (value >> 0x18);
//            this.OutStream.Write(this._buffer, 0, 4);
//        }

//        public int ReadInt32()
//        {
////    if (this.m_isMemoryStream)
////    {
////        MemoryStream stream = this.m_stream as MemoryStream;
////        return stream.InternalReadInt32();
////    }
////    this.FillBuffer(4);
////    return (((this.m_buffer[0] | (this.m_buffer[1] << 8)) | (this.m_buffer[2] << 0x10)) | (this.m_buffer[3] << 0x18));
////return (((this._buffer[num - 4] | (this._buffer[num - 3] << 8)) | (this._buffer[num - 2] << 0x10)) | (this._buffer[num - 1] << 0x18));

//        }
//    }
//}
