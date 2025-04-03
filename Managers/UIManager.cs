using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : SingletonMonoBehaviour<UIManager>
{


    /*
        FOR ANIMS, CREATE ANOTHER SCRIPT THAT MANAGES UI ANIMS
    */
    public bool isDebug;

    private InputSystem_Actions inputActions;
    private InputAction pause;
    
    [ReadOnly]
    public UI currentUI;

    public bool enableCursor = true;

    Dictionary<UI,UIBase> uiInstances = new Dictionary<UI,UIBase>();

    #region UIPrefabs

    [Title("Prefabs")] 
    public UIPause uiPausePrefab;
    public UIPhone uiPhonePrefab;
    #endregion


    [Title("Canvas Reference")]
    public Transform canvas;

    
    #region Input Actions
    private void OnEnable() {
        inputActions = new InputSystem_Actions();

        EnableUIInput();
        
        pause = inputActions.UI.Pause;
        pause.performed += ctx => {
            if(GameManager.instance.isPaused) {
                GameManager.instance.Resume();
                ShowUI(UI.GAMEPLAY);
            }
            else {
                ShowUI(UI.PAUSE);
                GameManager.instance.Pause();
            }
        };
    }

    private void OnDisable()
    {
        DisableUIInput();
    }

    public void EnableUIInput(){
        inputActions.UI.Enable();
    }

    public void DisableUIInput(){
        inputActions.UI.Disable();
    }

    #endregion

    
    void Start()
    {   
        InitializeUI(UI.PAUSE, uiPausePrefab);
        InitializeUI(UI.GAMEPLAY, null);

        HideUI(UI.PAUSE);
    }
    private void Update() {
        Cursor.visible = enableCursor;
        if(currentUI == UI.GAMEPLAY){
            Cursor.lockState = CursorLockMode.Locked;
        }
        else{
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void InitializeUI(UI ui, UIBase uiPrefab){
        if(uiPrefab != null){
            uiInstances[ui] = Instantiate(uiPrefab,canvas.transform);
        }
        else{
            uiInstances[ui] = null;
        }
    }
    public void ShowUI(UI ui){
        if(currentUI == ui){
            return;
        }

        HideUI(currentUI);

        if(uiInstances.TryGetValue(ui, out UIBase uiInstance)){
            uiInstance.Show();
        }

        currentUI = ui;
    }

    public void HideUI(UI ui){
        if(currentUI != ui){
            return;
        }
        if(uiInstances.TryGetValue(ui, out UIBase uiInstance)){
            uiInstance.Hide();
        }
        currentUI = UI.GAMEPLAY;
    }
    public enum UI{
        PAUSE,
        GAMEPLAY,
    }


}
