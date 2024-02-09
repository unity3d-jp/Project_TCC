using System.Collections.Generic;
using Unity.TinyCharacterController.Attributes;
using UnityEditor;
using UnityEngine;

namespace TinyCharacterControllerEditor.Attributes
{
//Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
//Altered by Brecht Lecluyse https://www.brechtos.com
 
[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorPropertyDrawer : PropertyDrawer
{
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);
 
            var attrib = this.attribute as TagSelectorAttribute;
 
            if (attrib.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
                //generate the taglist + custom tags
                List<string> tagList = new List<string>();
                tagList.Add("<NoTag>");
                tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);
                string propertyString = property.stringValue;
                int index = -1;
                if(propertyString =="")
                {
                    //The tag is empty
                    index = 0; //first index is the special <notag> entry
                }
                else
                {
                    //check if there is an entry that matches the entry and get the index
                    //we skip index 0 as that is a special custom case
                    for (int i = 1; i < tagList.Count; i++)
                    {
                        if (tagList[i] == propertyString)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                
                //Draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());
 
                //Adjust the actual string value of the property based on the selection
                if(index==0)
                {
                    property.stringValue = "";
                }
                else if (index >= 1)
                {
                    property.stringValue = tagList[index];
                }
                else
                {
                    property.stringValue = "";
                }
            }
 
            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
}