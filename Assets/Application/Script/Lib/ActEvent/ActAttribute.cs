using System;

namespace Project.Lib
{
	/// <summary>
	/// スキップしたときも実行するか
	/// </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExecuteSkipAttribute : Attribute
	{
		public ExecuteSkipAttribute(){
		}

	}
}
