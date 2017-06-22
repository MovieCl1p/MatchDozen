using System;
using UnityEngine;

namespace UI.Diagnostics
{
	public sealed class FPSCounterAttribute:DiagnosticAttribute
	{
	    private float timeA; 
	
	    private int fps;
	
	    private int lastFPS;
		
		public FPSCounterAttribute()
		{
			
		}
		
		public FPSCounterAttribute(DiagnosticColor color):base(color)
		{
			
		}
	
	    public override void OnStart () 
		{
			timeA = Time.timeSinceLevelLoad;
	    }
		
		public override string ToString ()
		{
			return string.Format ("FPS:{0}", lastFPS * Time.timeScale);
		}
	
		public override void OnUpdate ()
		{			
			if(Time.timeSinceLevelLoad  - timeA <= 1)
	        {
	            ++fps;
	        }
	        else
	        {
	            lastFPS = fps + 1;
				timeA = Time.timeSinceLevelLoad;
	            fps = 0;
	        }
			
			if(lastFPS*Time.timeScale > 30)
				Style.normal.textColor = Color.white;
			else
				Style.normal.textColor = Color.red;

		}
	}
}

