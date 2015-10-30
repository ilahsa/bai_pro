using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class IICPerformanceCounter
	{
		internal IICPerformanceCounter()
		{
		}

		internal IICPerformanceCounter(PerformanceCounter counter, PerformanceCounter baseCounter)
		{
			_counter = counter;
			_baseCounter = baseCounter;
		}

		public long Increment()
		{
			long result = 0;
			if (_counter != null)
				result = _counter.Increment();

			if (_baseCounter != null)
				_baseCounter.Increment();

			return result;
		}

		public long IncrementBy(long value)
		{
			long result = 0;
			if (_counter != null)
				result = _counter.IncrementBy(value);

			if (_baseCounter != null)
				_baseCounter.Increment();

			return result;
		}

		public long IncrementBy(long value, long baseValue)
		{
			long result = 0;
			if (_counter != null)
				result = _counter.IncrementBy(value);

			if (_baseCounter != null)
				_baseCounter.IncrementBy(baseValue);

			return result;
		}

		public long Decrement()
		{
			long result = 0;
			if (_counter != null)
				result = _counter.Decrement();

			if (_baseCounter != null)
				_baseCounter.Increment();

			return result;
		}

		public void Reset()
		{
			if (_counter != null)
				_counter.RawValue = 0L;

			if (_baseCounter != null)
				_baseCounter.RawValue = 0L;
		}

		public void IncreaseFraction(bool hitted)
		{
			if (_counter != null && hitted)
				_counter.Increment();

			if (_baseCounter != null)
				_baseCounter.Increment();
		}

		public void SetRawValue(long value)
		{
			if (_counter != null)
				_counter.RawValue = value;

			if (_baseCounter != null)
				_baseCounter.RawValue = value;
		}

		public void SetRawValue(long value, long baseValue)
		{
			if (_counter != null)
				_counter.RawValue = value;

			if (_baseCounter != null)
				_baseCounter.RawValue = baseValue;
		}

		public void Close()
		{
			if (_counter != null)
				_counter.Close();

			if (_baseCounter != null)
				_baseCounter.Close();
		}

		public string InstanceName
		{
			get
			{
				if (_counter == null)
					return string.Empty;
				else
					return _counter.InstanceName;
			}
			set
			{
				if (_counter != null)
					_counter.InstanceName = value;

				if (_baseCounter != null)
					_baseCounter.InstanceName = value;
			}
		}

		public void RemoveInstance()
		{
			if (_counter != null)
				_counter.RemoveInstance();

			if (_baseCounter != null)
				_baseCounter.RemoveInstance();
		}

		public bool Available
		{
			get { return _counter == null; } 
		}

		internal PerformanceCounter _counter;
		internal PerformanceCounter _baseCounter;

		internal IICPerformanceCounterAttribute _rawAttr;
		internal IICPerformanceCounterAttribute _baseAttr;
	}
}
