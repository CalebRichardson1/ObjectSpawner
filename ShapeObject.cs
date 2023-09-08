//Caleb Richardson, Interactive Scripting Lecture 3 Assignment, 9/6/2023

#region Summary
/// <summary>
/// Scriptable object that hold a spawned objects data, this data includes:
/// Object Spawn Chance
/// Object Shape
/// Object Scale Settings
/// Object Color Options
/// This scriptable object also uses a custom editor for easy of use and being designer friendly
/// </summary>
#endregion

using UnityEngine;
//if hash so that if we are using the unity editor, use the UnityEditor namespace
//otherwise the game will throw errors when we try to build
#if UNITY_EDITOR
using UnityEditor;
#endif

//allows us to have a create asset menu button that creates a new shape object scriptable object
[CreateAssetMenu(menuName = "Create New Shape Object", fileName = "New Shape Object")]

//scriptable objects are data containers that hold data about a object that we can reference in other scripts
public class ShapeObject : ScriptableObject
{
    //range property keeps the value inbetween these 2 values
    [Range(0f, 100f)] public float chanceToSpawn = 50f; //spawn chance of the object
    public ObjectShape shape; //enum with what object shape this object is
    public Mesh meshReference; //used if this object is a custom shape
    public bool randomScale; //boolean to say if this object should have a randomized scale
    public float scaleMin; //the minimum scale this object can be
    public float scaleMax; //the maximum scale this object can be
    public bool randomColor; //boolean to say if this object should be a random color
    public bool betweenHues; //boolean to say if this object's color should be a color between 2 hues
    public Color hue1; //if betweenHues is true this is the first hue the designer will decide on
    public Color hue2; //this is the second hue
    public Color shapeColor; //if both booleans are false then show what color should the shape should be
}

#region Editor
#if UNITY_EDITOR
[CustomEditor(typeof(ShapeObject))] //define what this custom editor is for
public class ShapeObjectEditor : Editor //notice we derive from editor rather than Monobehaviour, this is for using editor methods
{
    #region SerializedProperites 
    //define serialized properties for each of the variables in the Shape Object
    //this is so that this editor script will be able to change + save them for other scripts to use

    //spawn field
    SerializedProperty chanceToSpawn;

    //shape defintion fields
    SerializedProperty shape;
    SerializedProperty meshReference;

    //scale fields
    SerializedProperty randomScale;
    SerializedProperty scaleMin;
    SerializedProperty scaleMax;

    //color fields
    SerializedProperty randomColor;
    SerializedProperty betweenHues;
    SerializedProperty hue1;
    SerializedProperty hue2;
    SerializedProperty shapeColor;
    #endregion

    bool shapeScaleGroup, shapeColorGroup; //these booleans are for the foldout groups

    //when this editor script is enabled we need to reference each of the variables from the
    //shape object script, this is done with the .FindProperty() function
    private void OnEnable()
    {
        //serializedObject is a reference to the variable that this custom inspector is changing
        chanceToSpawn = serializedObject.FindProperty("chanceToSpawn");

        shape = serializedObject.FindProperty("shape");
        meshReference = serializedObject.FindProperty("meshReference");

        randomScale = serializedObject.FindProperty("randomScale");
        scaleMin = serializedObject.FindProperty("scaleMin");
        scaleMax = serializedObject.FindProperty("scaleMax");

        randomColor = serializedObject.FindProperty("randomColor");
        betweenHues = serializedObject.FindProperty("betweenHues");
        hue1 = serializedObject.FindProperty("hue1");
        hue2 = serializedObject.FindProperty("hue2");
        shapeColor = serializedObject.FindProperty("shapeColor");    
    }

    public override void OnInspectorGUI() //a virtual method that is used to create custom inspectors provided by unity
    {
        //we get a reference to the current object being inspected
        //target is the object that is being currently inspected
        //we cast target to the script we are try to get a reference to
        ShapeObject shapeObj = (ShapeObject)target;

        //we tell unity to update the object's values
        serializedObject.Update();

        //chance to spawn
        //labels are just simple pieces of text
        //editor styles are inbuild styles within unity
        GUILayout.Label("Spawn Options", EditorStyles.boldLabel);

        //PropertyField() is the reason why you can see [SerializedField] variables
        //property field exposes the variable in the editor so that we can edit it
        EditorGUILayout.PropertyField(chanceToSpawn);
        //Space() adds empty space vertically between GUI(graphical user interace) elements
        EditorGUILayout.Space(10f);

        //shape options
        GUILayout.Label("Shape Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shape);

        //if the shape of the object is of type custom we draw the mesh reference onto the GUI
        if(shapeObj.shape == ObjectShape.Custom)
        {
            //custom mesh popup code here
            EditorGUILayout.PropertyField(meshReference);
        }
        
        EditorGUILayout.Space(10f);

        #region Scale Foldout Group

        GUILayout.Label("Shape Scale Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(randomScale);

        if(shapeObj.randomScale) //if in the inspector we have randomScale set to true
        {
            //creates a foldout group we can put properties in
            //we set shapeScaleGroup boolean to it so that it can rollup/rolldown
            //by default we have shapeScaleGroup = false, this means the fold out group is rolled up,
            // this will be the case whenever we click off and back onto the scriptable object
            shapeScaleGroup = EditorGUILayout.BeginFoldoutHeaderGroup(shapeScaleGroup, "Shape Scale");

            //if the rollout group is true, we show our values, it becomes true when clicked on - this is handled by unity
            if(shapeScaleGroup)
            {
                float minValue = scaleMin.floatValue; //getting the float values from our serializedProperties
                float maxValue = scaleMax.floatValue;
                //we begin a horizontal group, meaning we have our GUI elements horizontal - side by side rather than vertically - up to down
                EditorGUILayout.BeginHorizontal(); 
                
                GUILayout.Label("Minimun Value");
                //we create a slider with for our scaleMin value, this is done with the Slider() method
                //we use GUIContent.none to hide the name of the values for more customization on the label of the value
                //MaxWidth() makes sure our sliders aren't to big for the inspector, and is able to be rendered side by side in the inspector
                EditorGUILayout.Slider(scaleMin, 0.1f, 30f, GUIContent.none, GUILayout.MaxWidth(150));

                GUILayout.Label("Maximum Value");
                //for the max scale we have our min value as the lowest point of the slider so our max can't go lower then our minimum
                EditorGUILayout.Slider(scaleMax, minValue, 30f, GUIContent.none, GUILayout.MaxWidth(150));
                //if our max value is less then the min value we set it equal to avoid the max being lower then the min
                if(maxValue < minValue) scaleMax.floatValue = scaleMin.floatValue;

                //we end the horizontal group and go back to the default vertical group
                EditorGUILayout.EndHorizontal();
            }
            //ending our foldout group
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        #endregion

        //color options
        EditorGUILayout.Space(10f);
        #region Color Foldout Group
        shapeColorGroup = EditorGUILayout.BeginFoldoutHeaderGroup(shapeColorGroup, "Shape Color Settings");
        if(shapeColorGroup)
        {
            EditorGUILayout.PropertyField(randomColor);
            if(!shapeObj.randomColor) //if in the inspector we have randomColor set to false
            {
                //show if random color false
                EditorGUILayout.PropertyField(betweenHues);
                if(shapeObj.betweenHues) //if in the inspector we have betweenHues set to true
                {
                    //show if between hues are true
                    EditorGUILayout.PropertyField(hue1);
                    EditorGUILayout.PropertyField(hue2);
                }

                //show if both color options are false
                else EditorGUILayout.PropertyField(shapeColor);  
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        //after modifing the values in the inspector, we need to apply the changes
        //otherwise they won't apply once you go into play mode
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion