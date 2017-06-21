

namespace UI.Diagnostics
{
	public sealed class MemoryCounterAttribute:DiagnosticAttribute
	{
		public MemoryCounterAttribute ()
		{
		}
		
		public override void OnUpdate ()
		{
			
		}
		
		public override void OnStart ()
		{
			
		}
		
		public override string ToString ()
		{
#if UNITY_IPHONE
			return EtceteraBinding.getMemoryStatistic();
#else			
			return string.Format("{0}",
			                     System.GC.GetTotalMemory(true) / 1024 / 1024);
#endif			
		}
	}
}

