using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class AnchorToolsEditor : EditorWindow
{
    private const string m_MENU_NAME = "Tools/AnchorSnapping";
    private static bool m_isEnable = false;

    [MenuItem(AnchorToolsEditor.m_MENU_NAME)]
    private static void ToggleAction()
    {
        /// Toggling action
        PerformAction(!AnchorToolsEditor.m_isEnable);
    }

    public static void PerformAction(bool enabled)
    {
        /// Set checkmark on menu item
        Menu.SetChecked(AnchorToolsEditor.m_MENU_NAME, enabled);
        /// Saving editor state
        EditorPrefs.SetBool(AnchorToolsEditor.m_MENU_NAME, enabled);

        AnchorToolsEditor.m_isEnable = enabled;
    }

    static AnchorToolsEditor()
    {
        AnchorToolsEditor.m_isEnable = EditorPrefs.GetBool(AnchorToolsEditor.m_MENU_NAME, false);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            PerformAction(AnchorToolsEditor.m_isEnable);
        };

        SceneView.onSceneGUIDelegate += OnScene;
    }

    private static void OnScene(SceneView sceneview)
    {
        if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && m_isEnable)
        {
            UpdateAnchors();
        }
    }

    public void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
    }

    static public Rect anchorRect;
    static public Vector2 anchorVector;
    static private Rect anchorRectOld;
    static private Vector2 anchorVectorOld;
    static private RectTransform currentRectTransform;
    static private RectTransform parentRectTransform;
    static private Vector2 pivotOld;
    static private Vector2 offsetMinOld;
    static private Vector2 offsetMaxOld;

    static public void UpdateAnchors()
    {
        TryToGetRectTransform();
        if (currentRectTransform != null && parentRectTransform != null && ShouldStick())
        {
            //Debug.Log("[Anchors Tools] Updating");
            Stick();
        }
    }

    static private bool ShouldStick()
    {
        return (
            currentRectTransform.offsetMin != offsetMinOld ||
            currentRectTransform.offsetMax != offsetMaxOld ||
            currentRectTransform.pivot != pivotOld ||
            anchorVector != anchorVectorOld ||
            anchorRect != anchorRectOld
            );
    }

    static private void Stick()
    {
        CalculateCurrentWH();
        CalculateCurrentXY();

        CalculateCurrentXY();
        pivotOld = currentRectTransform.pivot;
        anchorVectorOld = anchorVector;

        AnchorsToCorners();
        anchorRectOld = anchorRect;

        UnityEditor.EditorUtility.SetDirty(currentRectTransform.gameObject);
    }

    static private void TryToGetRectTransform()
    {
        currentRectTransform = UnityEditor.Selection.activeGameObject.GetComponent<RectTransform>();
        parentRectTransform = currentRectTransform.parent.gameObject.GetComponent<RectTransform>();

    }

    static private void CalculateCurrentXY()
    {
        float pivotX = anchorRect.width * currentRectTransform.pivot.x;
        float pivotY = anchorRect.height * (1 - currentRectTransform.pivot.y);
        Vector2 newXY = new Vector2(currentRectTransform.anchorMin.x * parentRectTransform.rect.width + currentRectTransform.offsetMin.x + pivotX - parentRectTransform.rect.width * anchorVector.x,
                                  -(1 - currentRectTransform.anchorMax.y) * parentRectTransform.rect.height + currentRectTransform.offsetMax.y - pivotY + parentRectTransform.rect.height * (1 - anchorVector.y));
        anchorRect.x = newXY.x;
        anchorRect.y = newXY.y;
        anchorRectOld = anchorRect;
    }

    static private void CalculateCurrentWH()
    {
        anchorRect.width = currentRectTransform.rect.width;
        anchorRect.height = currentRectTransform.rect.height;
        anchorRectOld = anchorRect;
    }

    static private void AnchorsToCorners()
    {
        float pivotX = anchorRect.width * currentRectTransform.pivot.x;
        float pivotY = anchorRect.height * (1 - currentRectTransform.pivot.y);
        currentRectTransform.anchorMin = new Vector2(0f, 1f);
        currentRectTransform.anchorMax = new Vector2(0f, 1f);
        currentRectTransform.offsetMin = new Vector2(anchorRect.x / currentRectTransform.localScale.x, anchorRect.y / currentRectTransform.localScale.y - anchorRect.height);
        currentRectTransform.offsetMax = new Vector2(anchorRect.x / currentRectTransform.localScale.x + anchorRect.width, anchorRect.y / currentRectTransform.localScale.y);
        currentRectTransform.anchorMin = new Vector2(currentRectTransform.anchorMin.x + anchorVector.x + (currentRectTransform.offsetMin.x - pivotX) / parentRectTransform.rect.width * currentRectTransform.localScale.x,
                                                 currentRectTransform.anchorMin.y - (1 - anchorVector.y) + (currentRectTransform.offsetMin.y + pivotY) / parentRectTransform.rect.height * currentRectTransform.localScale.y);
        currentRectTransform.anchorMax = new Vector2(currentRectTransform.anchorMax.x + anchorVector.x + (currentRectTransform.offsetMax.x - pivotX) / parentRectTransform.rect.width * currentRectTransform.localScale.x,
                                                 currentRectTransform.anchorMax.y - (1 - anchorVector.y) + (currentRectTransform.offsetMax.y + pivotY) / parentRectTransform.rect.height * currentRectTransform.localScale.y);
        currentRectTransform.offsetMin = new Vector2((0 - currentRectTransform.pivot.x) * anchorRect.width * (1 - currentRectTransform.localScale.x), (0 - currentRectTransform.pivot.y) * anchorRect.height * (1 - currentRectTransform.localScale.y));
        currentRectTransform.offsetMax = new Vector2((1 - currentRectTransform.pivot.x) * anchorRect.width * (1 - currentRectTransform.localScale.x), (1 - currentRectTransform.pivot.y) * anchorRect.height * (1 - currentRectTransform.localScale.y));

        offsetMinOld = currentRectTransform.offsetMin;
        offsetMaxOld = currentRectTransform.offsetMax;
    }
}
////This script must be placed in a folder called "Editor" in the root of the "Assets"
////Otherwise the script will not work as intended