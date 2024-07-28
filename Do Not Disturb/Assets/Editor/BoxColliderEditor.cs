using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoxCollider))]
public class BoxColliderEditor : Editor
{
    SerializedProperty centerProperty;
    SerializedProperty sizeProperty;

    void OnEnable()
    {
        // SerializedObject�� SerializedProperty �ʱ�ȭ �� ��ȿ�� Ȯ��
        if (serializedObject != null)
        {
            centerProperty = serializedObject.FindProperty("m_Center");
            sizeProperty = serializedObject.FindProperty("m_Size");

            // SerializedProperty ��ȿ�� �˻�
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
        // SerializedObject ��ȿ�� �˻�
        if (serializedObject == null)
        {
            Debug.LogError("SerializedObject is null in OnInspectorGUI.");
            return;
        }

        // SerializedObject ������Ʈ
        serializedObject.Update();

        // SerializedProperty ��ȿ�� �˻� �� PropertyField �׸���
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

        // ���� ���� ����
        serializedObject.ApplyModifiedProperties();
    }

    void OnDisable()
    {
        // �ʿ��� ��� ���� �۾� ����
        centerProperty = null;
        sizeProperty = null;
    }
}
