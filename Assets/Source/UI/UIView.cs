
using Core;

namespace UI
{
	abstract public class UIView : MonoScheduledBehaviour
	{
		
		protected override sealed void Start ()
		{
			OnStart();
		}
		
		
		protected sealed override void Update()
		{
			base.Update();
			
			OnUpdate();
		}
		
		protected virtual void OnUpdate()
		{
			
		}
		
		protected virtual void OnStart(){}		
	}
	
	
	abstract public class UIViewWithModel<T>:UIView, Observer
	{
		private T _model;

		public virtual T Model
		{
			get
			{
				return _model;
			}
			 set
			{
				if(_model != null && _model is Observable)
					(_model as Observable).RemoveObserver(this);
				
				_model = value;
				
				if(_model != null)
				{
					if(_model is Observable)
						(_model as Observable).AddObserver(this);
				
					ApplyModel();
					
					OnModelChanged();
				}
			}
		}

		protected sealed override void OnReleaseResources ()
		{
			base.OnReleaseResources ();
			ReleaseViewResources();
			this.Model = default(T);
		}

		protected virtual void ReleaseViewResources ()
		{
		}

		protected virtual void ApplyModel() {}
		protected virtual void OnModelChanged() {}
		
		#region Observer implementation
		public void OnObjectChanged (Observable observable)
		{
			OnModelChanged();
		}
		#endregion
	}
}