﻿using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Experimental.Perception.Randomization.Scenarios;
using UnityEngine.Experimental.Perception.Randomization.VisualElements;
using UnityEngine.UIElements;

namespace UnityEngine.Experimental.Perception.Randomization.Editor
{
    [CustomEditor(typeof(ScenarioBase), true)]
    class ScenarioBaseEditor : UnityEditor.Editor
    {
        ScenarioBase m_Scenario;
        SerializedObject m_SerializedObject;
        VisualElement m_Root;
        VisualElement m_InspectorPropertiesContainer;
        VisualElement m_ConstantsContainer;
        VisualElement m_RandomizerListPlaceholder;
        SerializedProperty m_ConstantsProperty;

        public override VisualElement CreateInspectorGUI()
        {
            m_Scenario = (ScenarioBase)target;
            m_SerializedObject = new SerializedObject(m_Scenario);
            m_Root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                $"{StaticData.uxmlDir}/ScenarioBaseElement.uxml").CloneTree();

            m_RandomizerListPlaceholder = m_Root.Q<VisualElement>("randomizer-list-placeholder");

            CreatePropertyFields();
            CheckIfConstantsExist();

            var serializeConstantsButton = m_Root.Q<Button>("serialize");
            serializeConstantsButton.clicked += () =>
            {
                m_Scenario.SerializeToFile();
                AssetDatabase.Refresh();
                var newConfigFileAsset = AssetDatabase.LoadAssetAtPath<Object>(m_Scenario.defaultConfigFileAssetPath);
                EditorGUIUtility.PingObject(newConfigFileAsset);
            };

            var deserializeConstantsButton = m_Root.Q<Button>("deserialize");
            deserializeConstantsButton.clicked += () =>
            {
                Undo.RecordObject(m_Scenario, "Deserialized scenario configuration");
                m_Scenario.DeserializeFromFile(m_Scenario.defaultConfigFilePath);
            };

            return m_Root;
        }

        void CreatePropertyFields()
        {
            m_InspectorPropertiesContainer = m_Root.Q<VisualElement>("inspector-properties");
            m_InspectorPropertiesContainer.Clear();

            var iterator = m_SerializedObject.GetIterator();
            var foundProperties = false;
            if (iterator.NextVisible(true))
            {
                do
                {
                    switch (iterator.name)
                    {
                        case "m_Script":
                            break;
                        case "constants":
                            m_ConstantsProperty = iterator.Copy();
                            break;
                        case "m_Randomizers":
                            m_RandomizerListPlaceholder.Add(new RandomizerList(iterator.Copy()));
                            break;
                        default:
                        {
                            foundProperties = true;
                            var propertyField = new PropertyField(iterator.Copy());
                            propertyField.Bind(m_SerializedObject);
                            m_InspectorPropertiesContainer.Add(propertyField);
                            break;
                        }
                    }
                } while (iterator.NextVisible(false));
            }

            if (!foundProperties)
                m_InspectorPropertiesContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        void CheckIfConstantsExist()
        {
            m_ConstantsContainer = m_Root.Q<VisualElement>("constants-container");
            if (m_ConstantsProperty == null)
            {
                m_InspectorPropertiesContainer.style.marginBottom = 0;
                m_ConstantsContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }
    }
}
