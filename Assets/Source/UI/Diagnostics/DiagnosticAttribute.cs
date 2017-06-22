using System;
using UnityEngine;

namespace UI.Diagnostics
{
	public enum DiagnosticColor
	{
		Blue,
		White
	}
	
	public abstract class DiagnosticAttribute:Attribute
	{		
		
		public GUIStyle Style
		{
			get;
			protected set;
		}
		
		public DiagnosticAttribute():this(DiagnosticColor.White)
		{ 			
			
		}
		
		public DiagnosticAttribute(DiagnosticColor color)
		{
			Style = new GUIStyle();	
			
			
			if(color == DiagnosticColor.Blue)
				Style.normal.textColor = Color.blue;
			else
				Style.normal.textColor = Color.white;
		}

		public abstract void OnUpdate();	
		public abstract void OnStart();
	}	
}