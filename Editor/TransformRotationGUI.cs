using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
	internal sealed class TransformRotationGUI
	{
		//================================================================================
		// 変数（readonly）
		//================================================================================
		private readonly object     m_instance;
		private readonly MethodInfo m_onEnableMethod;
		private readonly MethodInfo m_rotationFieldMethod;

		//================================================================================
		// 関数
		//================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TransformRotationGUI()
		{
			var type = Type.GetType( "UnityEditor.TransformRotationGUI,UnityEditor" );

			m_onEnableMethod      = type.GetMethod( "OnEnable" );
			m_rotationFieldMethod = type.GetMethod( "RotationField", new Type[] { } );

			m_instance = Activator.CreateInstance( type );
		}

		/// <summary>
		/// 有効になった時に呼び出します
		/// </summary>
		public void OnEnable( SerializedProperty property )
		{
			var parameters = new object[] { property, new GUIContent( string.Empty ) };
			m_onEnableMethod.Invoke( m_instance, parameters );
		}

		/// <summary>
		/// Inspector の GUI を描画します
		/// </summary>
		public void RotationField()
		{
			m_rotationFieldMethod.Invoke( m_instance, null );
		}
	}
}