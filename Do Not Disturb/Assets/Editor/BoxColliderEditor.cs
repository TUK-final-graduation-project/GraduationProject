using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoxCollider))]
public class BoxColliderEditor : Editor
{
    SerializedProperty centerProperty;
    SerializedProperty sizeProperty;

    void OnEnable()
    {
        // SerializedObject와 SerializedProperty 초기화 및 유효성 확인
        if (serializedObject != null)
        {
            centerProperty = serializedObject.FindProperty("m_Center");
            sizeProperty = serializedObject.FindProperty("m_Size");

            // SerializedProperty 유효성 검사
            if (centerProperty == null)
            {
                Debug.LogError("Center property not found.");
            }

            if (sizeProperty == null)
            {
                Debug.LogError("Size property not found.");
            }
        }
        else
        {
            Debug.LogError("SerializedObject is null.");
        }
    }

    public override void OnInspectorGUI()
    {
        // SerializedObject 유효성 검사
        if (serializedObject == null)
        {
            Debug.LogError("SerializedObject is null in OnInspectorGUI.");
            return;
        }

        // SerializedObject 업데이트
        serializedObject.Update();

        // SerializedProperty 유효성 검사 및 PropertyField 그리기
        if (centerProperty != null && centerProperty.serializedObject.targetObject != null)
        {
            EditorGUILayout.PropertyField(centerProperty, new GUIContent("Center"));
        }
        else
        {
            EditorGUILayout.LabelField("Center property not found or has been disposed.");
        }

        if (sizeProperty != null && sizeProperty.serializedObject.targetObject != null)
        {
            EditorGUILayout.PropertyField(sizeProperty, new GUIContent("Size"));
        }
        else
        {
            EditorGUILayout.LabelField("Size property not found or has been disposed.");
        }

        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }

    void OnDisable()
    {
        // 필요한 경우 해제 작업 수행
        centerProperty = null;
        sizeProperty = null;
    }
}
