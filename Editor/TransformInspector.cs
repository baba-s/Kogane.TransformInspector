using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
	[CanEditMultipleObjects]
	[CustomEditor( typeof( Transform ) )]
	internal sealed class TransformInspector : Editor
	{
		//================================================================================
		// 定数（const）
		//================================================================================
		private const BindingFlags SET_LOCAL_EULER_ANGLES_ATTR =
			BindingFlags.Instance |
			BindingFlags.NonPublic;

		//================================================================================
		// 定数（static readonly）
		//================================================================================
		private static readonly object[]        RESET_ROTATION_PARAMETERS = { Vector3.zero, 0 };
		private static readonly GUIContent      PROPERTY_FIELD_LABEL      = new GUIContent( string.Empty );
		private static readonly GUILayoutOption RESET_BUTTON_OPTION       = GUILayout.Width( 20 );

		//================================================================================
		// 変数
		//================================================================================
		private SerializedProperty   m_positionProperty;
		private SerializedProperty   m_rotationProperty;
		private SerializedProperty   m_scaleProperty;
		private TransformRotationGUI m_transformRotationGUI;
		private MethodInfo           m_setLocalEulerAnglesMethod;

		//================================================================================
		// 関数
		//================================================================================
		/// <summary>
		/// 有効になった時に呼び出されます
		/// </summary>
		private void OnEnable()
		{
			m_positionProperty = serializedObject.FindProperty( "m_LocalPosition" );
			m_rotationProperty = serializedObject.FindProperty( "m_LocalRotation" );
			m_scaleProperty    = serializedObject.FindProperty( "m_LocalScale" );

			if ( m_transformRotationGUI == null )
			{
				m_transformRotationGUI = new TransformRotationGUI();
			}

			m_transformRotationGUI.OnEnable( m_rotationProperty );

			if ( m_setLocalEulerAnglesMethod == null )
			{
				var transformType = typeof( Transform );
				m_setLocalEulerAnglesMethod = transformType.GetMethod
				(
					name: "SetLocalEulerAngles",
					bindingAttr: SET_LOCAL_EULER_ANGLES_ATTR
				);
			}
		}

		/// <summary>
		/// Inspector の GUI を描画する時に呼び出されます
		/// </summary>
		public override void OnInspectorGUI()
		{
			var oldLabelWidth = EditorGUIUtility.labelWidth;

			// プロパティのラベルの表示幅を設定
			if ( !EditorGUIUtility.wideMode )
			{
				EditorGUIUtility.wideMode   = true;
				EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
			}

			serializedObject.Update();

			// 位置の入力欄を表示
			using ( new EditorGUILayout.HorizontalScope() )
			{
				// リセットボタン
				if ( GUILayout.Button( "P", RESET_BUTTON_OPTION ) )
				{
					m_positionProperty.vector3Value = Vector3.zero;
				}

				// 入力欄
				EditorGUILayout.PropertyField( m_positionProperty, PROPERTY_FIELD_LABEL );
			}

			// 回転角の入力欄を表示
			using ( new EditorGUILayout.HorizontalScope() )
			{
				// リセットボタン
				if ( GUILayout.Button( "R", RESET_BUTTON_OPTION ) )
				{
					var targetObjects = m_rotationProperty.serializedObject.targetObjects;

					Undo.RecordObjects( targetObjects, "Inspector" );

					foreach ( var n in targetObjects )
					{
						m_setLocalEulerAnglesMethod.Invoke( n, RESET_ROTATION_PARAMETERS );
					}
				}

				// 入力欄
				m_transformRotationGUI.RotationField();
			}

			// スケーリング値の入力欄を表示
			using ( new EditorGUILayout.HorizontalScope() )
			{
				// リセットボタン
				if ( GUILayout.Button( "S", RESET_BUTTON_OPTION ) )
				{
					m_scaleProperty.vector3Value = Vector3.one;
				}

				// 入力欄
				EditorGUILayout.PropertyField( m_scaleProperty, PROPERTY_FIELD_LABEL );
			}

			// 四捨五入ボタンを表示
			using ( new EditorGUILayout.HorizontalScope() )
			{
				EditorGUILayout.LabelField( "Round", GUILayout.MaxWidth( 96 ) );

				var isEnablePosition = m_positionProperty.vector3Value.HasAfterDecimalPoint();
				var isEnableRotation = m_rotationProperty.quaternionValue.eulerAngles.HasAfterDecimalPoint();
				var isEnableScale    = m_scaleProperty.vector3Value.HasAfterDecimalPoint();

				var oldEnable = GUI.enabled;

				GUI.enabled = isEnablePosition;

				if ( GUILayout.Button( "P" ) )
				{
					RoundPosition();
				}

				GUI.enabled = isEnableRotation;

				if ( GUILayout.Button( "R" ) )
				{
					RoundRotation();
				}

				GUI.enabled = isEnableScale;

				if ( GUILayout.Button( "S" ) )
				{
					RoundScale();
				}

				GUI.enabled = isEnablePosition || isEnableRotation || isEnableScale;

				if ( GUILayout.Button( "All" ) )
				{
					RoundPosition();
					RoundRotation();
					RoundScale();
				}

				GUI.enabled = oldEnable;
			}

			// 変更を反映
			serializedObject.ApplyModifiedProperties();

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}

		private void RoundPosition()
		{
			m_positionProperty.vector3Value = m_positionProperty.vector3Value.Round();
		}

		private void RoundRotation()
		{
			var eulerAngles = m_rotationProperty.quaternionValue.eulerAngles;
			eulerAngles                        = eulerAngles.Round();
			m_rotationProperty.quaternionValue = Quaternion.Euler( eulerAngles );
		}

		private void RoundScale()
		{
			m_scaleProperty.vector3Value = m_scaleProperty.vector3Value.Round();
		}
	}
}