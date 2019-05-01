using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KoganeUnityLibEditor
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( Transform ) )]
    public sealed class TransformInspector : Editor
    {
        //==============================================================================
        // 列挙型
        //==============================================================================
        private enum TargetType
        {
            POSITION,
            ROTATION,
            SCALE,
        }

        //==============================================================================
        // 変数
        //==============================================================================
        private GUIStyle m_buttonStyle;

        //==============================================================================
        // 関数
        //==============================================================================
        public override void OnInspectorGUI()
        {
            if ( m_buttonStyle == null )
            {
                m_buttonStyle = new GUIStyle( EditorStyles.toolbarButton )
                {
                    fixedHeight = 20,
                    fixedWidth  = 20,
                };
            }

            serializedObject.Update();

            var transform = target as Transform;

            DrawLine( "P", TargetType.POSITION, transform );
            DrawLine( "R", TargetType.ROTATION, transform );
            DrawLine( "S", TargetType.SCALE, transform );

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLine( string label, TargetType type, Transform transform )
        {
            var newValue = Vector3.zero;
            var isReset  = false;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();

            if ( GUILayout.Button( label, m_buttonStyle ) )
            {
                newValue = type == TargetType.SCALE ? Vector3.one : Vector3.zero;
                isReset  = true;
            }

            if ( !isReset )
            {
                switch ( type )
                {
                    case TargetType.POSITION:
                        newValue = Vector3Field( transform.localPosition );
                        break;
                    case TargetType.ROTATION:
                        newValue = Vector3Field( transform.localEulerAngles );
                        break;
                    case TargetType.SCALE:
                        newValue = Vector3Field( transform.localScale );
                        break;
                }
            }

            EditorGUILayout.EndHorizontal();

            if ( !EditorGUI.EndChangeCheck() && !isReset ) return;

            Undo.RecordObjects
            (
                targets,
                $"{( isReset ? "Reset" : "Change" )} {transform.gameObject.name} {type.ToString()}"
            );

            foreach ( var t in targets.OfType<Transform>() )
            {
                switch ( type )
                {
                    case TargetType.POSITION:
                        t.localPosition = newValue;
                        break;
                    case TargetType.ROTATION:
                        t.localEulerAngles = newValue;
                        break;
                    case TargetType.SCALE:
                        t.localScale = newValue;
                        break;
                }

                EditorUtility.SetDirty( t );
            }
        }

        private static Vector3 Vector3Field( Vector3 value )
        {
            return EditorGUILayout.Vector3Field( string.Empty, value, GUILayout.Height( 16 ) );
        }
    }
}