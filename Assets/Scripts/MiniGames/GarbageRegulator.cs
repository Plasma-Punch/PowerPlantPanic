using UnityEngine;
using System;

public class GarbageRegulator : MonoBehaviour, IMiniGame
{
    [SerializeField]
    private GameObject _itemHolder;
    [SerializeField]
    private GameObject _barrelPrefab;
    [SerializeField]
    private GameObject _barrelSpawnLocation;
    [SerializeField]
    private GameObject _barrelPickUpTrigger;
    [SerializeField]
    private GameObject _barrelPlaceTrigger;
    [SerializeField]
    private GameEvent _minigameFinished;

    [Header("Audio Variables")]
    [SerializeField]
    private AudioClip _grabSound;
    [SerializeField]
    private AudioClip _placeSound;


    private SoundManager _soundManager;

    private GameObject _heldItem;
    private GameObject _spawnedBarrel;

    private void OnEnable()
    {
        if (GameObject.Find("SoundManager") != null)
            _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        else
            Debug.Log("SoundManager not found");
    }

    private void Start()
    {
        _soundManager.LoadSoundWithOutPath("grab", _grabSound);
        _soundManager.LoadSoundWithOutPath("place", _placeSound);
    }

    public void completed()
    {
        _minigameFinished.Raise(this, new MiniGameFinishedEventArgs { FinishedMiniGame = MiniGame.WasteManagement});
    }

    public void failed()
    {

    }

    public void StartMiniGame(Component sender, object obj)
    {
        SpawnBarrel();
    }
    private void SpawnBarrel()
    {
        if (_spawnedBarrel != null) return;
        GameObject go = Instantiate(_barrelPrefab, _barrelSpawnLocation.transform.position, _barrelPrefab.transform.rotation);
        _spawnedBarrel = go;
        _barrelPickUpTrigger.SetActive(true);
    }

    public void OpenWasteControl(Component sender, object obj)
    {
        StartMiniGame(sender, obj);
    }

    public void PickUpBarrel(Component sender, object obj)
    {
        if (_heldItem != null) return;

        _soundManager.SetSFXVolume(1f);
        _soundManager.PlaySound("grab");

        _heldItem = _spawnedBarrel;
        _heldItem.transform.parent = _itemHolder.transform;
        _heldItem.transform.localPosition = Vector3.zero;
        _barrelPickUpTrigger.SetActive(false);
        _barrelPlaceTrigger.SetActive(true);
    }

    public void PlaceBarrel(Component sender, object obj)
    {
        if (sender.transform.parent.gameObject.transform.parent.gameObject != gameObject) return;
        if (_heldItem == null) return;

        _soundManager.SetSFXVolume(1);
        _soundManager.PlaySound("place");

        Destroy(_heldItem);
        _spawnedBarrel = null;
        _barrelPlaceTrigger.SetActive(false);
        completed();
    }
}
