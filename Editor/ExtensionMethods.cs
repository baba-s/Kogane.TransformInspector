using System;
using UnityEngine;

namespace Kogane.Internal
{
	internal static class Vector3Ext
	{
		//================================================================================
		// 関数（static）
		//================================================================================
		public static bool HasAfterDecimalPoint( this float self )
		{
			return 0.000001f < Math.Abs( self % 1 );
		}

		public static bool HasAfterDecimalPoint( this Vector3 self )
		{
			return
				self.x.HasAfterDecimalPoint() ||
				self.y.HasAfterDecimalPoint() ||
				self.z.HasAfterDecimalPoint()
				;
		}

		public static Vector3 Round( this Vector3 self )
		{
			return new Vector3
			(
				Mathf.Round( self.x ),
				Mathf.Round( self.y ),
				Mathf.Round( self.z )
			);
		}
	}
}